<template>
  <div class="app-container">
    <h3>{{ $t('Test Server Spec') }}</h3>
    <el-card class="box-card">
      <el-form label-width="40%" size="mini">
        <el-form-item :label="$t('Total Memory')">
          <span>{{ device && device.totalMem + ' MB' }}</span>
        </el-form-item>
        <el-form-item :label="$t('Available Memory')">
          <span>{{ device && device.availableMem + ' MB' }}</span>
        </el-form-item>
        <el-form-item :label="$t('Cpu Model')">
          <span>{{ device && device.cpuModel }}</span>
        </el-form-item>
        <el-form-item :label="$t('Cpu Cores')">
          <span>{{ device && device.cpuCores }}</span>
        </el-form-item>
        <el-form-item :label="$t('Cpu Frequency')">
          <span>{{ device && device.cpuFrequency + ' MHz' }}</span>
        </el-form-item>
        <el-form-item :label="$t('Cpu Cache Size')">
          <span>{{ device && device.cpuCacheSize + ' KB' }}</span>
        </el-form-item>
      </el-form>
    </el-card>
    <el-row type="flex" style="margin-top:10px;" justify="end">
      <el-button type="primary" icon="el-icon-refresh" @click="refresh">{{ $t('Refresh') }}</el-button>
    </el-row>
  </div>
</template>

<script>
import { getDevice, refreshDevice } from '@/api/device'

export default {
  data() {
    return {
      device: null
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    fetchData() {
      getDevice().then(response => {
        this.device = response
      })
    },
    refresh() {
      refreshDevice().then(response => {
        this.device = response
      })
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Test Server Spec": "Test Server Spec",
    "Total Memory": "Total Memory:",
    "Available Memory": "Available Memory:",
    "Cpu Model": "Cpu Model:",
    "Cpu Cores": "Cpu Cores:",
    "Cpu Frequency": "Cpu Frequency:",
    "Cpu Cache Size": "Cpu Cache Size:",
    "Refresh": "Refresh"
  },
  "zh": {
    "Test Server Spec": "测试服务器配置",
    "Total Memory": "总内存:",
    "Available Memory": "可用内存:",
    "Cpu Model": "Cpu 型号:",
    "Cpu Cores": "Cpu 核数:",
    "Cpu Frequency": "Cpu 主频:",
    "Cpu Cache Size": "Cpu 缓存:",
    "Refresh": "刷新"
  }
}
</i18n>
