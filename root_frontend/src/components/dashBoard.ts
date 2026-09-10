import { computed, defineComponent, onMounted, onUnmounted, ref } from "vue";

// Scala massima di ogni sensore: serve a convertire il valore in percentuale
// per el-progress (che lavora da 0 a 100).
const SCALE = {
    rpm: 8000, // giri/min
    velocita: 260, // km/h
    temperaturaLiquido: 150, // °C
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
        };
    },
});