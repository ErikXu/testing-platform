<template>
  <div class="app-container">
    <h3>Test Server Device Spec</h3>
    <el-card class="box-card">
      <el-form label-width="40%" size="mini">
        <el-form-item label="Total Memory:">
          <span>{{ device && device.totalMem + ' MB' }}</span>
        </el-form-item>
        <el-form-item label="Available Memory:">
          <span>{{ device && device.availableMem + ' MB' }}</span>
        </el-form-item>
        <el-form-item label="Cpu Model:">
          <span>{{ device && device.cpuModel }}</span>
        </el-form-item>
        <el-form-item label="Cpu Cores:">
          <span>{{ device && device.cpuCores }}</span>
        </el-form-item>
        <el-form-item label="Cpu Frequency:">
          <span>{{ device && device.cpuFrequency + ' MHz' }}</span>
        </el-form-item>
        <el-form-item label="Cpu Cache Size:">
          <span>{{ device && device.cpuCacheSize + ' KB' }}</span>
        </el-form-item>
      </el-form>
    </el-card>
    <el-row type="flex" style="margin-top:10px;" justify="end">
      <el-button type="primary" icon="el-icon-refresh" @click="refresh">Refresh</el-button>
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
