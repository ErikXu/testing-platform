<template>
  <div class="app-container">
    <h3>{{ $t('Scene Details') }}</h3>
    <el-card class="box-card">
      <el-form label-width="40%" size="mini">
        <el-form-item :label="$t('Id')">
          <span>{{ detail && detail.id }}</span>
        </el-form-item>
        <el-form-item :label="$t('Name')">
          <span>{{ detail && detail.name }}</span>
        </el-form-item>
        <el-form-item :label="$t('Url')">
          <span>{{ detail && detail.url }}</span>
        </el-form-item>
        <el-form-item :label="$t('Method')">
          <span>{{ detail && detail.method }}</span>
        </el-form-item>
        <el-form-item :label="$t('Thread')">
          <span>{{ detail && detail.thread }}</span>
        </el-form-item>
        <el-form-item :label="$t('Connection')">
          <span>{{ detail && detail.connection }}</span>
        </el-form-item>
        <el-form-item :label="$t('Duration')">
          <span>{{ detail && (detail.duration + detail.unit) }}</span>
        </el-form-item>
      </el-form>
    </el-card>
    <h3>{{ $t('Task List') }}</h3>
    <el-table
      :data="tasks"
      border
      fit
      highlight-current-row
    >
      <el-table-column label="#" align="center" width="55">
        <template slot-scope="scope">
          {{ scope.$index + 1 }}
        </template>
      </el-table-column>
      <el-table-column :label="$t('ID')" align="left" width="220">
        <template slot-scope="scope">
          <span>{{ scope.row.id }}</span>
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
      <el-table-column :label="$t('Monitor')" align="center" width="90">
        <template slot-scope="{row}">
          <el-button v-if="row.status === 1" type="success" size="mini" @click="monitor(row)">
            {{ $t('View') }}
          </el-button>
          <el-button v-else type="success" size="mini" disabled>
            {{ $t('View') }}
          </el-button>
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
import { getScene, getTaskOfScene } from '@/api/scene'

export default {
  data() {
    return {
      detail: null,
      tasks: []
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    fetchData() {
      const id = this.$route.params.id
      getScene(id).then(response => {
        this.detail = response
      })
      getTaskOfScene(id).then(response => {
        this.tasks = response
      })
    },
    report(row) {
      this.$router.push({ name: 'task-report', params: { id: row.id }})
    },
    monitor(row) {
      var url = window.location.protocol + '//' + window.location.hostname + ':8080/?id=' + row.id
      window.open(url)
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Scene Details": "Scene Details",
    "Id": "Id:",
    "Name": "Name:",
    "Url": "Url:",
    "Method": "Method:",
    "Thread": "Thread:",
    "Connection": "Connection:",
    "Duration": "Duration:",
    "Task List": "Task List",
    "ID": "ID",
    "Status": "Status",
    "IsBaseline": "IsBaseline",
    "StartTime": "StartTime",
    "EndTime": "EndTime",
    "CreationTime": "CreationTime",
    "Monitor": "Monitor",
    "Report": "Report",
    "View": "View",
    "Waiting": "Waiting",
    "Runing": "Runing",
    "Done": "Done",
    "Error": "Error"
  },
  "zh": {
    "Scene Details": "场景详情",
    "Id": "Id:",
    "Name": "名称:",
    "Url": "Url:",
    "Method": "Method:",
    "Thread": "线程数:",
    "Connection": "连接数:",
    "Duration": "持续时间:",
    "Task List": "任务列表",
    "ID": "ID",
    "Status": "状态",
    "IsBaseline": "基线版本",
    "StartTime": "开始时间",
    "EndTime": "结束时间",
    "CreationTime": "创建时间",
    "Monitor": "监控",
    "Report": "报告",
    "View": "查看",
    "Waiting": "等待中",
    "Runing": "运行中",
    "Done": "已完成",
    "Error": "已失败"
  }
}
</i18n>
