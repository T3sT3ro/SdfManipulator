#!/usr/bin/env bash
# Copies the dev project's Assets/Examples into the package's Samples~/Examples, .meta files included, so asset
# references inside the sample keep working after users import it. Examples is the source of truth: edit it in the
# dev project, then run this script and commit both.
#
#   tools/sync-samples.sh           sync
#   tools/sync-samples.sh --check   exit 1 if Samples~/Examples differs from Assets/Examples (for CI)
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
src="$root/projects/SdfManipulator/Assets/Examples"
dst="$root/packages/me.tooster.sdf/Samples~/Examples"

if [[ "${1:-}" == "--check" ]]; then
    if diff -r -q "$src" "$dst"; then
        echo "Samples~/Examples is in sync with Assets/Examples"
    else
        echo "Samples~/Examples is out of date; run tools/sync-samples.sh" >&2
        exit 1
    fi
    exit 0
fi

rm -rf "${dst:?}"
mkdir -p "$(dirname "$dst")"
cp -a "$src" "$dst"
echo "Synced $src -> $dst"
