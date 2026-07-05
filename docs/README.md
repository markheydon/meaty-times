# End-user documentation (GitHub Pages)

This folder is the **published source** for the Meaty Times end-user documentation site, built with [Jekyll](https://jekyllrb.com/) and hosted on [GitHub Pages](https://docs.github.com/en/pages).

**Audience:** Home cooks and anyone using Meaty Times who needs help, privacy information, or support guidance.

**This folder is not for:**

- Build, deployment, or architecture documentation
- API contracts, Spec Kit artefacts, or contributor workflows
- Repository-specific technical reference

**Developer and contributor documentation lives elsewhere:**

| Location | Purpose |
|----------|---------|
| [`docs-internal/`](../docs-internal/) | Internal developer documentation |
| [`README.md`](../README.md) | Repository overview and local development |
| [`CONTRIBUTING.md`](../CONTRIBUTING.md) | How to contribute |
| [`specs/`](../specs/) | Feature specifications and plans |

## Local preview

From this directory:

```powershell
bundle install
bundle exec jekyll serve --baseurl=""
```

Open [http://127.0.0.1:4000](http://127.0.0.1:4000). See [Testing your GitHub Pages site locally with Jekyll](https://docs.github.com/en/pages/setting-up-a-github-pages-site-with-jekyll/testing-your-github-pages-site-locally-with-jekyll?platform=windows) for prerequisites (Ruby and Bundler).

If you use Ruby 3.0 or later and see a `webrick` error, run `bundle add webrick` and try again.

## Publishing

GitHub Pages publishes from the `/docs` folder on the default branch. The custom domain is configured in [`CNAME`](CNAME).
