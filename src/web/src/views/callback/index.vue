<template>
  <div class="app-container">
    <h3>{{ $t('Callback Setting') }}</h3>
    <el-form :inline="true" :model="callback">
      <el-form-item :label="$t('Stress Test Callback Enabled')">
        <el-switch v-model="callback.isStressTestEnabled" @change="switchStressTest" />
      </el-form-item>
      <el-form-item :label="$t('Api Test Callback Enabled')">
        <el-switch v-model="callback.isApiTestEnabled" @change="switchApiTest" />
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import { getCallback, switchStressTest, switchApiTest } from '@/api/callback'
export default {
  data() {
    return {
      callback: null
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    fetchData() {
      getCallback().then(response => {
        this.callback = response
      })
    },
    switchStressTest() {
      switchStressTest(this.callback.id).then(() => {
        this.fetchData()
      })
    },
    switchApiTest() {
      switchApiTest(this.callback.id).then(() => {
        this.fetchData()
      })
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Callback Setting": "Callback Setting",
    "Stress Test Callback Enabled": "Stress Test Callback Enabled",
    "Api Test Callback Enabled": "Api Test Callback Enabled"
  },
  "zh": {
    "Callback Setting": "回调设置",
    "Stress Test Callback Enabled": "允许压力测试回调",
    "Api Test Callback Enabled": "允许接口测试回调"
  }
}
</i18n>
