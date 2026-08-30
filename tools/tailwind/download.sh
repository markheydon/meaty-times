#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARGET="$SCRIPT_DIR/tailwindcss"
VERSION="${TAILWIND_CLI_VERSION:-v4.3.3}"

if [[ -x "$TARGET" ]]; then
  exit 0
fi

OS="$(uname -s | tr '[:upper:]' '[:lower:]')"
ARCH="$(uname -m)"

case "$OS-$ARCH" in
  linux-x86_64|linux-amd64) ASSET="tailwindcss-linux-x64" ;;
  linux-aarch64|linux-arm64) ASSET="tailwindcss-linux-arm64" ;;
  darwin-x86_64|darwin-amd64) ASSET="tailwindcss-macos-x64" ;;
  darwin-arm64) ASSET="tailwindcss-macos-arm64" ;;
  *)
    echo "Unsupported platform: $OS-$ARCH" >&2
    exit 1
    ;;
esac

URL="https://github.com/tailwindlabs/tailwindcss/releases/download/${VERSION}/${ASSET}"
echo "Downloading Tailwind CLI ${VERSION} (${ASSET})..."
curl -fsSL "$URL" -o "$TARGET"
chmod +x "$TARGET"
