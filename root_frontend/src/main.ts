import { createApp } from 'vue';
import { createPinia } from 'pinia';
import ElementPlus from 'element-plus';
import 'element-plus/dist/index.css';
import it from 'element-plus/es/locale/lang/it';
import App from './App.vue';
import router from './router';
import { ElButton } from 'element-plus';

ElButton.setPropsDefaults({
    type: 'primary',
    size: 'small',
});

const app = createApp(App);

app.use(createPinia());
app.use(router);
app.use(ElementPlus, { locale: it });

app.mount('#app')

