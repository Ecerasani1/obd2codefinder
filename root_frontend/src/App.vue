<template>
  <el-config-provider :locale="it">
    <dashBoard></dashBoard>
  </el-config-provider>
</template>

<style scoped></style>
<script setup lang="ts">

import dashBoard from './components/dashBoard.vue';
import { ElConfigProvider } from 'element-plus';
import it from 'element-plus/es/locale/lang/it';

</script>
