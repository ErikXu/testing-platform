<template>
  <div class="app-container">
    <h3>{{ $t('Stress Test Task') }}</h3>
    <el-table
      :data="list"
      border
      fit
      highlight-current-row
      style="margin-top:10px;"
    >
      <el-table-column label="#" align="center" width="55">
        <template slot-scope="scope">
          {{ scope.$index + 1 }}
        </template>
      </el-table-column>
      <el-table-column :label="$t('Scene')" align="left" width="140">
        <template slot-scope="{row}">
          <el-button type="text" @click="toScene(row)">{{ row.sceneName }}</el-button>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Status')" align="center" width="80">
        <template slot-scope="scope">
          <el-tag v-if="scope.row.status === 0" type="info" size="small">{{ $t('Waiting') }}</el-tag>
          <el-tag v-else-if="scope.row.status === 1" type="primary" size="small">{{ $t('Runing') }}</el-tag>
          <el-tag v-else-if="scope.row.status === 2" type="success" size="small">{{ $t('Done') }}</el-tag>
          <el-tag v-else-if="scope.row.status === 3" type="danger" size="small">{{ $t('Error') }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Baseline')" align="center" width="100">
        <template slot-scope="scope">
          <el-switch v-model="scope.row.isBaseline" disabled />
        </template>
      </el-table-column>
      <el-table-column :label="$t('From')" align="center" width="100">
        <template slot-scope="scope">
          <el-tag v-if="scope.row.from === 0" type="primary" size="small">{{ $t('Console') }}</el-tag>
          <el-tag v-else-if="scope.row.from === 1" type="primary" size="small">{{ $t('Callback') }}</el-tag>
          <el-tag v-else-if="scope.row.from === 2" type="primary" size="small">{{ $t('Schedule') }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('StartTime')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.startRunningTime | simpleFormat }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('EndTime')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.endRunningTime | simpleFormat }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Creation Time')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.creationTime | simpleFormat }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Report')" align="center" width="90">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="report(row)">
            {{ $t('View') }}
          </el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { getTaskList } from '@/api/stress-task'

export default {
  name: 'Task',
  data() {
    return {
      list: []
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    fetchData() {
      getTaskList().then(response => {
        this.list = response
      })
    },
    toScene(row) {
      this.$router.push({ name: 'stress-scene-detail', params: { id: row.sceneId }})
    },
    report(row) {
      this.$router.push({ name: 'stress-task-report', params: { id: row.id }})
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Stress Test Task": "Stress Test - Task",
    "Scene": "Scene",
    "Status": "Status",
    "Baseline": "Baseline",
    "StartTime": "StartTime",
    "EndTime": "EndTime",
    "Creation Time": "Creation Time",
    "Report": "Report",
    "View": "View",
    "Waiting": "Waiting",
    "Runing": "Runing",
    "Done": "Done",
    "Error": "Error",
    "Console": "Console",
    "Callback": "Callback",
    "Schedule": "Schedule"
  },
  "zh": {
    "Stress Test Task": "压力测试 - 任务",
    "Scene": "场景",
    "Status": "状态",
    "Baseline": "基线版本",
    "StartTime": "开始时间",
    "EndTime": "结束时间",
    "Creation Time": "创建时间",
    "Report": "报告",
    "View": "查看",
    "Waiting": "等待中",
    "Runing": "运行中",
    "Done": "已完成",
    "Error": "已失败",
    "Console": "控制台",
    "Callback": "回调",
    "Schedule": "定时任务"
  }
}
</i18n>
