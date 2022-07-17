<template>
  <div class="app-container">
    <h3 style="margin-bottom:0px;">{{ $t('Scene Details') }}</h3>
    <el-row type="flex" style="margin-bottom:10px;" justify="end">
      <el-button type="success" size="mini" @click="run">{{ $t('Run') }}</el-button>
      <el-button type="primary" size="mini" @click="back">{{ $t('Back') }}</el-button>
    </el-row>
    <el-card class="box-card">
      <el-form label-width="30%" size="mini">
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
        <el-form-item :label="$t('Content Type')">
          <span>{{ detail && detail.contentType }}</span>
        </el-form-item>
        <el-form-item :label="$t('Body')">
          <span>{{ detail && detail.body }}</span>
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
        <el-form-item :label="$t('Callback Address')">
          <span>{{ getCallbackAddress() }}</span> <el-button type="success" size="mini" @click="test">{{ $t('Test') }}</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    <h3 style="margin-bottom:0px;">{{ $t('Task List') }}</h3>
    <el-row type="flex" style="margin-bottom:10px;" justify="end">
      <el-button type="primary" size="mini" @click="refresh">{{ $t('Refresh') }}</el-button>
    </el-row>
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
      <el-table-column :label="$t('ID')" align="left" width="210">
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
      <el-table-column :label="$t('From')" align="center" width="100">
        <template slot-scope="scope">
          <el-tag v-if="scope.row.from === 0" type="primary" size="small">{{ $t('Console') }}</el-tag>
          <el-tag v-else-if="scope.row.from === 1" type="primary" size="small">{{ $t('Callback') }}</el-tag>
          <el-tag v-else-if="scope.row.from === 2" type="primary" size="small">{{ $t('Schedule') }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Baseline')" align="center" width="80">
        <template slot-scope="scope">
          <el-switch v-model="scope.row.isBaseline" @change="switchBaseline(scope.row)" />
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
      <el-table-column :label="$t('Monitor')" align="center" width="80">
        <template slot-scope="{row}">
          <el-button v-if="row.status === 1" type="success" size="mini" @click="monitor(row)">
            {{ $t('View') }}
          </el-button>
          <el-button v-else type="success" size="mini" disabled>
            {{ $t('View') }}
          </el-button>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Report')" align="center" width="80">
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
import { getScene, getTaskOfScene } from '@/api/stress-scene'
import { switchBaseline, addTask } from '@/api/stress-task'
import request from '@/utils/request'

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
    switchBaseline(row) {
      switchBaseline(row.id).then(() => {
        this.fetchData()
      })
    },
    report(row) {
      this.$router.push({ name: 'stress-task-report', params: { id: row.id }})
    },
    getCallbackAddress() {
      return 'http://' + window.location.host + '/api/callbacks/stress-test?sceneId=' + this.detail.id + '&caller=test'
    },
    monitor(row) {
      var url = window.location.protocol + '//' + window.location.hostname + ':8080/?id=' + row.id
      window.open(url)
    },
    refresh() {
      this.fetchData()
    },
    run() {
      addTask(this.detail.id).then(() => {
        this.$message({
          type: 'success',
          message: 'Run success!'
        })
        this.fetchData()
      })
    },
    back() {
      this.$router.push({ name: 'stress-scene' })
    },
    test() {
      var url = this.getCallbackAddress()
      return request({
        url: url,
        method: 'get'
      }).then(() => {
        this.$message({
          type: 'success',
          message: 'Test success!'
        })
        this.fetchData()
      })
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
    "Content Type": "Content Type:",
    "Body": "Body:",
    "Thread": "Thread:",
    "Connection": "Connection:",
    "Duration": "Duration:",
    "Callback Address": "Callback Address:",
    "Task List": "Task List",
    "ID": "ID",
    "Status": "Status",
    "From": "From",
    "Baseline": "Baseline",
    "StartTime": "StartTime",
    "EndTime": "EndTime",
    "Creation Time": "Creation Time",
    "Monitor": "Monitor",
    "Report": "Report",
    "View": "View",
    "Waiting": "Waiting",
    "Runing": "Runing",
    "Done": "Done",
    "Error": "Error",
    "Refresh": "Refresh",
    "Run": "Run",
    "Back": "Back",
    "Test": "Test",
    "Console": "Console",
    "Callback": "Callback",
    "Schedule": "Schedule"
  },
  "zh": {
    "Scene Details": "场景详情",
    "Id": "Id:",
    "Name": "名称:",
    "Url": "Url:",
    "Method": "Method:",
    "Content Type": "Content Type:",
    "Body": "Body:",
    "Thread": "线程数:",
    "Connection": "连接数:",
    "Duration": "持续时间:",
    "Callback Address": "回调地址:",
    "Task List": "任务列表",
    "ID": "ID",
    "Status": "状态",
    "From": "来源",
    "Baseline": "基线版本",
    "StartTime": "开始时间",
    "EndTime": "结束时间",
    "Creation Time": "创建时间",
    "Monitor": "监控",
    "Report": "报告",
    "View": "查看",
    "Waiting": "等待中",
    "Runing": "运行中",
    "Done": "已完成",
    "Error": "已失败",
    "Refresh": "刷新",
    "Run": "运行",
    "Back": "返回",
    "Test": "测试",
    "Console": "控制台",
    "Callback": "回调",
    "Schedule": "定时任务"
  }
}
</i18n>
