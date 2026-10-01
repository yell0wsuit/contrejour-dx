"""Puts tools/ on sys.path so tests import `webcontent` and `publish_browser` as the scripts do."""

import sys
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[2]
REPO = TOOLS.parent
sys.path.insert(0, str(TOOLS))
