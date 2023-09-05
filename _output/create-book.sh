npm install -g md-to-pdf
mkdir -p _output
for f in ./**/*.md; do cat $f; echo; done | md-to-pdf --stylesheet "../theme/style.css" > ../_output/blue-paper.pdf