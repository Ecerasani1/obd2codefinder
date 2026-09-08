# Fonte dei dati DTC

I codici DTC contenuti in `dtcCode.json` e nella cartella `Brands/` provengono
dal progetto open source:

- **Repository**: https://github.com/Wal33D/dtc-database
- **Licenza**: MIT
- **Dataset**: tabella `dtc_definitions` del database `data/dtc_codes.db`

I codici generici (SAE J2012) sono in `dtcCode.json`; i codici specifici di
marca sono in `Brands/<marca>.json`.

Lo script che rigenera questi file è in `tools/generate_dtc_data.py` (nella
radice del backend).
