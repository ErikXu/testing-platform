<template>
  <div class="app-container">
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
        <template slot-scope="scope">
          <span>{{ scope.row.sceneName }}</span>
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
      <el-table-column :label="$t('IsBaseline')" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.isBaseline }}</span>
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
      <el-table-column :label="$t('CreationTime')" align="left">
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
    report(row) {
      this.$router.push({ name: 'task-report', params: { id: row.id }})
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Scene": "Scene",
    "Status": "Status",
    "IsBaseline": "IsBaseline",
    "StartTime": "StartTime",
    "EndTime": "EndTime",
    "CreationTime": "CreationTime",
    "Report": "Report",
    "View": "View",
    "Waiting": "Waiting",
    "Runing": "Runing",
    "Done": "Done",
    "Error": "Error"
  },
  "zh": {
    "Scene": "场景",
    "Status": "状态",
    "IsBaseline": "基线版本",
    "StartTime": "开始时间",
    "EndTime": "结束时间",
    "CreationTime": "创建时间",
    "Report": "报告",
    "View": "查看",
    "Waiting": "等待中",
    "Runing": "运行中",
    "Done": "已完成",
    "Error": "已失败"
  }
}
</i18n>
