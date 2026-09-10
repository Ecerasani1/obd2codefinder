<template>
    <div class="dashboard">
        <el-tabs v-model="tabAttiva" type="border-card">
            <el-tab-pane label="Motore" name="motore">
                <el-row :gutter="16">
                    <el-col :span="6">
                        <el-card shadow="hover" class="gauge">
                            <template #header>Giri motore</template>
                            <el-progress type="dashboard" :percentage="rpmPercent" :width="160">
                                <span class="gauge__value">{{ rpm }} <small class="gauge__unit">g/min</small></span>
                            </el-progress>
                        </el-card>
                    </el-col>
                    <el-col :span="6">
                        <el-card shadow="hover" class="gauge">
                            <template #header>Velocità</template>
                            <el-progress type="dashboard" :percentage="velocitaPercent" :width="160">
                                <span class="gauge__value">{{ velocita }} <small class="gauge__unit">km/h</small></span>
                            </el-progress>
                        </el-card>
                    </el-col>
                    <el-col :span="6">
                        <el-card shadow="hover" class="gauge">
                            <template #header>Temperatura liquido</template>
                            <el-progress type="circle" :percentage="temperaturaPercent" :width="160"
                                :color="temperaturaColor">
                                <span class="gauge__value">{{ temperaturaLiquido }} <small
                                        class="gauge__unit">°C</small></span>
                            </el-progress>
                        </el-card>
                    </el-col>
                    <el-col :span="6">
                        <el-card shadow="hover" class="gauge">
                            <template #header>Carico motore</template>
                            <el-progress type="circle" :percentage="caricoMotore" :width="160">
                                <span class="gauge__value">{{ caricoMotore }} <small
                                        class="gauge__unit">%</small></span>
                            </el-progress>
                        </el-card>
                    </el-col>
                </el-row>
            </el-tab-pane>
            <el-tab-pane label="Errori" name="errori">
                <el-empty v-if="codiciErrore.length === 0" description="Nessun errore rilevato" />
                <el-row v-else :gutter="16">
                    <el-col v-for="dtc in codiciErrore" :key="dtc.code" :span="24">
                        <el-card shadow="hover" class="dtc">
                            <div class="dtc__row">
                                <el-tag :type="TAG_TYPE_PER_SEVERITY[dtc.severity]" effect="dark" class="dtc__code">
                                    {{ dtc.code }}
                                </el-tag>
                                <span class="dtc__description">
                                    {{ dtc.description ?? "Codice non riconosciuto" }}
                                </span>
                            </div>
                        </el-card>
                    </el-col>
                </el-row>
            </el-tab-pane>
            <el-tab-pane label="Diagnosi" name="diagnosi">
                <el-empty v-if="diagnosiOrdinata.length === 0" description="Nessuna diagnosi disponibile" />
                <el-collapse v-else v-model="pannelliAperti" class="diagnosi">
                    <el-collapse-item v-for="gruppo in diagnosiOrdinata" :key="gruppo.dtc.code" :name="gruppo.dtc.code">
                        <template #title>
                            <div class="diagnosi__head">
                                <el-tag :type="TAG_TYPE_PER_SEVERITY[gruppo.dtc.severity]" effect="dark"
                                    class="diagnosi__code">
                                    {{ gruppo.dtc.code }}
                                </el-tag>
                                <span class="diagnosi__title">
                                    {{ gruppo.dtc.description ?? "Codice non riconosciuto" }}
                                </span>
                                <span class="diagnosi__top">{{ gruppo.topProbability }}%</span>
                            </div>
                        </template>
                        <div v-for="ipotesi in gruppo.hypotheses" :key="ipotesi.component" class="diagnosi__item">
                            <div class="diagnosi__label">
                                <span>{{ ipotesi.component }}</span>
                                <span class="diagnosi__percent">{{ ipotesi.probability }}%</span>
                            </div>
                            <el-progress :percentage="ipotesi.probability" :stroke-width="12" :show-text="false" />
                        </div>
                    </el-collapse-item>
                </el-collapse>
            </el-tab-pane>
        </el-tabs>
    </div>
</template>
<script lang="ts" src="./dashBoard.ts"></script>
<style scoped lang="less" src="./style.less"></style>
