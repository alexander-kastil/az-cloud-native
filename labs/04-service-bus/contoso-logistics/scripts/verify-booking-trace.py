import json
import os
import shutil
import subprocess
import sys
import time
import urllib.error
import urllib.request

SHIPMENT_ID = "3f2c9a1e-7b4d-4e8a-9c21-5d6f0b8e4a17"
TRACKING_NUMBER = "CL-2026-000142"
APPHOST = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "src", "AppHost")
ASPIRE = shutil.which("aspire") or "aspire"
FAILED = []


def run(*args, timeout=300):
    return subprocess.run([ASPIRE, *args, "--non-interactive", "--nologo"], capture_output=True, text=True, timeout=timeout)


def check(name, ok, detail=""):
    print(f"{'PASS' if ok else 'FAIL'} {name}" + (f": {detail}" if detail else ""))
    if not ok:
        FAILED.append(name)
    return ok


def http_url(resource):
    for url in resource.get("urls") or []:
        if not url.get("isInternal") and url["url"].startswith("http://"):
            return url["url"].rstrip("/")
    for url in resource.get("urls") or []:
        if not url.get("isInternal") and url["url"].startswith("http"):
            return url["url"].rstrip("/")
    return None


def request(method, url, body=None):
    data = json.dumps(body).encode() if body is not None else None
    req = urllib.request.Request(url, data=data, method=method, headers={"Content-Type": "application/json"})
    try:
        with urllib.request.urlopen(req, timeout=30) as response:
            return response.status
    except urllib.error.HTTPError as error:
        return error.code
    except (urllib.error.URLError, OSError):
        return 0


def json_from(text):
    start = min((i for i in (text.find("["), text.find("{")) if i >= 0), default=0)
    return json.loads(text[start:])


def main():
    started = run("start", "--apphost", APPHOST, timeout=500)
    if not check("aspire start", started.returncode == 0, started.stderr.strip()[-200:]):
        return

    for name in ("shipping-api", "tracking-service"):
        waited = run("wait", name, "--apphost", APPHOST, "--timeout", "180", timeout=240)
        if not check(f"{name} healthy", waited.returncode == 0, waited.stderr.strip()[-200:]):
            return

    described = run("describe", "--apphost", APPHOST, "--format", "Json")
    resources = json_from(described.stdout)
    if isinstance(resources, dict):
        resources = resources.get("resources", [])
    urls = {r["displayName"]: http_url(r) for r in resources if r.get("displayName") in ("shipping-api", "tracking-service")}
    if not check("resolve urls", all(urls.get(n) for n in ("shipping-api", "tracking-service")), json.dumps(urls)):
        return

    status = request("POST", f"{urls['shipping-api']}/shipments", {"shipmentId": SHIPMENT_ID, "trackingNumber": TRACKING_NUMBER})
    if not check("POST /shipments returns 201", status == 201, f"got {status}"):
        return

    deadline = time.monotonic() + 60
    status = 0
    while time.monotonic() < deadline:
        status = request("GET", f"{urls['tracking-service']}/tracking/{TRACKING_NUMBER}")
        if status == 200:
            break
        time.sleep(2)
    check("GET /tracking returns 200", status == 200, f"got {status}")

    traces = []
    found = False
    sources = set()
    while time.monotonic() < deadline + 30:
        result = run("otel", "traces", "--apphost", APPHOST, "--format", "Json", "--limit", "100")
        traces = json_from(result.stdout) if result.returncode == 0 and result.stdout.strip() else []
        if isinstance(traces, dict):
            traces = traces.get("traces", [])
        for trace in traces:
            if trace.get("title") == "POST /shipments":
                sources = {span.get("source") for span in trace.get("spans", [])}
                if {"shipping-api", "tracking-service"} <= sources:
                    found = True
        if found:
            break
        time.sleep(3)
    check("trace POST /shipments spans shipping-api and tracking-service", found, f"sources {sorted(s for s in sources if s)}")


try:
    main()
finally:
    stopped = run("stop", "--apphost", APPHOST)
    print(f"{'PASS' if stopped.returncode == 0 else 'FAIL'} aspire stop")

sys.exit(1 if FAILED else 0)
