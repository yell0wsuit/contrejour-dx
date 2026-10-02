"""Copies the OFL license texts beside the subset fonts. The license travels with the font; the game
never reads them, so they are not preloaded."""

from __future__ import annotations

import shutil
from pathlib import Path

from . import layout, pipeline, progress


def _copy(job: pipeline.Job) -> None:
    shutil.copyfile(job.source, job.out_path)


def copy_licenses(
    content_root: Path,
    out_root: Path,
    entries: dict[str, str],
    report: progress.Reporter = progress.SILENT,
) -> tuple[int, int]:
    jobs = []
    for source in sorted((content_root / "Content/fonts/licenses").glob("*")):
        if source.is_file() and not source.name.startswith("."):
            relative = layout.output_relative(layout.relative_to(source, content_root))
            jobs.append(pipeline.Job(source, relative.as_posix(), out_root / relative, "copy"))
    return pipeline.run_stage("licenses", jobs, _copy, entries, report, cpu_bound=False)
