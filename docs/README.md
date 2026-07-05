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

You can preview the site with native Ruby or inside a Linux container (recommended on Windows when Ruby install or IT policy gets in the way).

### Option A — Podman (recommended on Windows)

Requires [Podman](https://podman.io/). Gems install inside a Linux container, so you do not need Ruby on the host and you avoid Windows-specific Bundler path issues.

From the repository root:

```powershell
# Live preview at http://127.0.0.1:4000
.\scripts\Start-JekyllDocsSite.ps1

# One-off production build (output in docs/_site)
.\scripts\Start-JekyllDocsSite.ps1 -BuildOnly
```

Stop the preview server with `Ctrl+C`. Wait for `Server running...` in the console before opening the URL — the first run installs gems and can take a minute.

If you have a Compose provider installed (`podman-compose` or Docker Compose), you can alternatively use [`compose.yml`](compose.yml) from this directory:

```powershell
cd docs
podman compose up
podman compose --profile build run --rm build
```

The container uses `ruby:3.3-bookworm`, your `Gemfile` / `Gemfile.lock`, and the same `github-pages` gem set GitHub uses. Bundle gems are stored in a Podman volume (`meaty-times-jekyll-bundle`), separate from any local `vendor/bundle` folder.

If the browser reports **connection refused**, check that the container is still running (`podman ps`) and that you waited for Jekyll to finish starting. The preview binds to `127.0.0.1` on the host port (not only inside the Podman VM).

### Option B — Native Ruby

From this directory:

```powershell
bundle install
bundle exec jekyll serve --baseurl=""
```

Open [http://127.0.0.1:4000](http://127.0.0.1:4000). See [Testing your GitHub Pages site locally with Jekyll](https://docs.github.com/en/pages/setting-up-a-github-pages-site-with-jekyll/testing-your-github-pages-site-locally-with-jekyll?platform=windows) for prerequisites (Ruby and Bundler).

If you use Ruby 3.0 or later and see a `webrick` error, run `bundle add webrick` and try again.

If native `bundle install` fails on Windows (for example Rouge or path errors under `vendor/bundle`), delete `vendor/bundle` and use **Option A** instead.

## Publishing

GitHub Pages publishes from the `/docs` folder on the default branch. The custom domain is configured in [`CNAME`](CNAME).
