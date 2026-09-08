#!/usr/bin/env python3
"""
Genera i file JSON dei codici DTC per obdCodeFinder.

Fonte dati: https://github.com/Wal33D/dtc-database (licenza MIT)
Database:  data/dtc_codes.db (tabella dtc_definitions)

Output:
  - Data/dtcCode.json           -> codici "universali": GENERIC + OTHER
  - Data/Brands/<marca>.json    -> codici specifici per marca (minuscolo)
"""
import json
import sqlite3
from pathlib import Path

DB_PATH = Path("/tmp/dtc-src/dtc_codes.db")
OUT_DIR = (
    Path(__file__).resolve().parent.parent
    / "obdCodeFinder/obdCodeFinder.Handlers/Data"
)  # obdCodeFinder.Handlers/Data


def main() -> None:
    con = sqlite3.connect(DB_PATH)
    cur = con.cursor()
    cur.execute(
        "SELECT code, manufacturer, description FROM dtc_definitions "
        "WHERE locale = 'en'"
    )

    generici: dict[str, str] = {}
    marche: dict[str, dict[str, str]] = {}

    for code, manufacturer, description in cur.fetchall():
        description = description.strip()
        if manufacturer in ("GENERIC", "OTHER"):
            generici[code] = description
        else:
            marca = manufacturer.lower()
            marche.setdefault(marca, {})[code] = description

    # Scrive i generici
    (OUT_DIR / "dtcCode.json").write_text(
        json.dumps(generici, indent=2, ensure_ascii=False), encoding="utf-8"
    )

    # Scrive un file per marca
    brands_dir = OUT_DIR / "Brands"
    brands_dir.mkdir(exist_ok=True)
    for marca, codici in sorted(marche.items()):
        (brands_dir / f"{marca}.json").write_text(
            json.dumps(codici, indent=2, ensure_ascii=False), encoding="utf-8"
        )

    print(f"Generici (GENERIC+OTHER): {len(generici)}")
    print(f"Marche: {len(marche)}")
    for marca, codici in sorted(marche.items()):
        print(f"  {marca}: {len(codici)}")


if __name__ == "__main__":
    main()
