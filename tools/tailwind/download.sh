#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TARGET="$SCRIPT_DIR/tailwindcss"
VERSION="${TAILWIND_CLI_VERSION:-v4.3.3}"

OS="$(uname -s | tr '[:upper:]' '[:lower:]')"
ARCH="$(uname -m)"

case "$OS-$ARCH" in
  linux-x86_64|linux-amd64)
    ASSET="tailwindcss-linux-x64"
    EXPECTED_SHA256="dc61b3ac6b8c9ca874c0cc4c57b2409791a64c5540404ca5f5367360babc313a"
    ;;
  linux-aarch64|linux-arm64)
    ASSET="tailwindcss-linux-arm64"
    EXPECTED_SHA256="55fd0b241214eff3de1e8ee4f22796662f2d2e7a49bcfca7477cfd0bac398195"
    ;;
  darwin-x86_64|darwin-amd64)
    ASSET="tailwindcss-macos-x64"
    EXPECTED_SHA256="7922e0953f2110c05976e3bf58f14e643d90427575e766b7d433f5f80cbee7e1"
    ;;
  darwin-arm64)
    ASSET="tailwindcss-macos-arm64"
    EXPECTED_SHA256="cdf646702987a743464dff4d9c60fd4480d1c1e73dd819a9a67f1078815dce9d"
    ;;
  *)
    echo "Unsupported platform: $OS-$ARCH" >&2
    exit 1
    ;;
esac

verify_checksum() {
  echo "${EXPECTED_SHA256}  ${TARGET}" | sha256sum -c --status
}

if [[ -x "$TARGET" ]] && verify_checksum; then
  exit 0
fi

if [[ -f "$TARGET" ]]; then
  echo "Existing Tailwind CLI failed checksum verification; re-downloading..." >&2
  rm -f "$TARGET"
fi

URL="https://github.com/tailwindlabs/tailwindcss/releases/download/${VERSION}/${ASSET}"
echo "Downloading Tailwind CLI ${VERSION} (${ASSET})..."
curl -fsSL "$URL" -o "$TARGET"
chmod +x "$TARGET"

if ! verify_checksum; then
  echo "Downloaded Tailwind CLI failed checksum verification." >&2
  rm -f "$TARGET"
  exit 1
fi
