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
        </el-tabs>
    </div>
</template>
<script lang="ts" src="./dashBoard.ts"></script>
<style scoped lang="less" src="./style.less"></style>https://element-plus.org/en-US/component/overview