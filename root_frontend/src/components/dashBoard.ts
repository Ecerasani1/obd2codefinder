import { computed, defineComponent, onMounted, onUnmounted, ref } from "vue";

// Scala massima di ogni sensore: serve a convertire il valore in percentuale
// per el-progress (che lavora da 0 a 100).
const SCALE = {
    rpm: 8000, // giri/min
    velocita: 260, // km/h
    temperaturaLiquido: 150, // °C
};

// --- Tipi di dominio -------------------------------------------------------
// NB: devono rispecchiare il contratto del backend (Core/Models).
// La gravità è un valore di DOMINIO (high/medium/low): NON sa nulla di Element Plus.
export type DtcSeverity = "high" | "medium" | "low";

export interface Dtc {
    code: string;
    // Può essere null: il backend restituisce null se il codice non è nel dizionario
    description: string | null;
    severity: DtcSeverity;
}

// La diagnosi è un concetto separato dal codice errore:
// non "questo errore è grave", ma "probabilmente va sostituito questo componente".
export interface DiagnosisHypothesis {
    component: string;
    probability: number; // 0-100
}

// Raggruppamento per la UI: un DTC con le sue ipotesi.
export interface DtcDiagnosis {
    dtc: Dtc;
    hypotheses: DiagnosisHypothesis[];
    // Probabilita' della migliore ipotesi, calcolata qui:
    // cosi' il template non deve accedere a hypotheses[0] (che TS vede come forse undefined).
    topProbability: number;
}

// --- Traduzione dominio -> presentazione -----------------------------------
type ElTagType = "primary" | "success" | "info" | "warning" | "danger";

// Qui, e SOLO qui, decidiamo che "high" si pittura di rosso.
// È una scelta grafica, e vive nel frontend.
const TAG_TYPE_PER_SEVERITY: Record<DtcSeverity, ElTagType> = {
    high: "danger",
    medium: "warning",
    low: "success",
};

export default defineComponent({
    name: "dashBoard",
    setup() {
        // Valori "finti" per ora: quando collegheremo la presa OBD
        // arriveranno in tempo reale da un servizio.
        const tabAttiva = ref("motore");
        const rpm = ref(2500);
        const velocita = ref(87);
        const temperaturaLiquido = ref(93);
        const caricoMotore = ref(45);

        // Dati finti in attesa del backend.
        const codiciErrore = ref<Dtc[]>([
            { code: "P0301", description: "Mancata accensione rilevata - cilindro 1", severity: "high" },
            { code: "P0128", description: "Termostato: temperatura sotto la soglia di regolazione", severity: "medium" },
            { code: "P0171", description: "Miscela troppo magra (banco 1)", severity: "medium" },
            { code: "P0442", description: "Piccola perdita nel sistema di recupero vapori", severity: "low" },
            { code: "P0XYZ", description: null, severity: "medium" }, // caso "non riconosciuto"
        ]);

        // Le ipotesi sono indicizzate PER CODICE, non duplicate dentro il DTC:
        // cosi' i codici restano in una sola fonte di verita' (codiciErrore).
        const ipotesiPerCodice = ref<Record<string, DiagnosisHypothesis[]>>({
            P0301: [
                { component: "Candela cilindro 1", probability: 72 },
                { component: "Bobina di accensione", probability: 18 },
                { component: "Iniettore cilindro 1", probability: 10 },
            ],
            P0128: [
                { component: "Sensore temperatura liquido", probability: 85 },
                { component: "Termostato bloccato aperto", probability: 15 },
            ],
            P0171: [
                { component: "Filtro aria intasato", probability: 40 },
                { component: "Sonda lambda", probability: 35 },
                { component: "Perdita d'aria aspirata", probability: 25 },
            ],
        });

        // Fisarmonica: quali pannelli sono aperti (di default il migliore).
        const pannelliAperti = ref<string[]>(["P0301"]);

        // Uniamo codici + ipotesi, ordiniamo le ipotesi per probabilita' decrescente
        // e ordiniamo i gruppi in base alla loro ipotesi piu' probabile.
        const diagnosiOrdinata = computed<DtcDiagnosis[]>(() =>
            codiciErrore.value
                .map((dtc) => ({
                    dtc,
                    hypotheses: [...(ipotesiPerCodice.value[dtc.code] ?? [])].sort(
                        (a, b) => b.probability - a.probability,
                    ),
                }))
                .filter((gruppo) => gruppo.hypotheses.length > 0)
                .map((gruppo) => ({
                    ...gruppo,
                    topProbability: gruppo.hypotheses[0]?.probability ?? 0,
                }))
                .sort((a, b) => b.topProbability - a.topProbability),
        );

        let timerId: number | undefined;

        const rpmPercent = computed(() => (rpm.value / SCALE.rpm) * 100);
        const velocitaPercent = computed(() => (velocita.value / SCALE.velocita) * 100);
        const temperaturaPercent = computed(() => (temperaturaLiquido.value / SCALE.temperaturaLiquido) * 100);
        // Se il motore è troppo caldo il cerchio diventa rosso
        const temperaturaColor = computed(() => (temperaturaLiquido.value >= 110 ? "#f56c6c" : "#67c23a"));

        // Simulazione presa OBD: ogni secondo i valori si muovono piano piano,
        // restando dentro intervalli "realistici".
        const aggiornaValori = () => {
            rpm.value = Math.min(6500, Math.max(800, rpm.value + (Math.random() - 0.5) * 300));
            velocita.value = Math.min(180, Math.max(0, velocita.value + (Math.random() - 0.5) * 10));
            temperaturaLiquido.value = Math.min(120, Math.max(70, temperaturaLiquido.value + (Math.random() - 0.5) * 2));
            caricoMotore.value = Math.min(100, Math.max(0, caricoMotore.value + (Math.random() - 0.5) * 8));
        };

        onMounted(() => {
            timerId = window.setInterval(aggiornaValori, 1000);
        });

        onUnmounted(() => {
            if (timerId !== undefined) {
                window.clearInterval(timerId);
            }
        });

        return {
            tabAttiva,
            rpm,
            velocita,
            temperaturaLiquido,
            caricoMotore,
            rpmPercent,
            velocitaPercent,
            temperaturaPercent,
            temperaturaColor,
            codiciErrore,
            diagnosiOrdinata,
            pannelliAperti,
            TAG_TYPE_PER_SEVERITY,
        };
    },
});