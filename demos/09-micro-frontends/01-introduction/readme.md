# Introduction to Micro Frontends

Micro Frontends is an architectural style that is inspired by Microservices. It is an approach to developing web applications as a composition of small client-side applications. Each application is owned by a small team, has its own codebase, and is completely independent of other applications. The applications can be written in different frameworks, can be deployed separately, and can be composed into a single user experience.

In Angular we can implement Micro Frontends using Module Federation. Module Federation is  feature that allows us to dynamically load code from a remote application at runtime. This allows us to update our application without having to redeploy the entire application.