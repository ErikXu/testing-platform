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
        <el-form-item :label="$t('Description')">
          <span>{{ detail && detail.description }}</span>
        </el-form-item>
        <el-form-item :label="$t('Url')">
          <span>{{ detail && detail.url }}</span>
        </el-form-item>
        <el-form-item :label="$t('Method')">
          <el-tag v-if="detail && detail.method === 'GET'" size="small" effect="dark" color="#61AFFE" style="border-color: #d9ecff;">{{ detail && detail.method }}</el-tag>
          <el-tag v-if="detail && detail.method === 'POST'" size="small" effect="dark" color="#49C990" style="border-color: #d9ecff;">{{ detail && detail.method }}</el-tag>
          <el-tag v-if="detail && detail.method === 'PUT'" size="small" effect="dark" color="#FCA130" style="border-color: #d9ecff;">{{ detail && detail.method }}</el-tag>
          <el-tag v-if="detail && detail.method === 'PATCH'" size="small" effect="dark" color="#50E3C2" style="border-color: #d9ecff;">{{ detail && detail.method }}</el-tag>
          <el-tag v-if="detail && detail.method === 'DELETE'" size="small" effect="dark" color="#F93E3E" style="border-color: #d9ecff;">{{ detail && detail.method }}</el-tag>
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

    <h3>{{ $t('Agent') }}</h3>
    <el-row type="flex" style="margin-bottom:10px;" justify="end">
      <el-button size="mini" type="success" @click="addAgent">{{ $t('Add') }}</el-button>
      <el-button size="mini" type="primary" @click="refreshAgents">{{ $t('Refresh') }}</el-button>
    </el-row>
    <el-table
      :data="agentsOfScene"
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
      <el-table-column :label="$t('Address')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.agentAddress }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Port')" align="left">
        <template slot-scope="{row}">
          <span>{{ row.agentPort }} </span> <el-button type="text" style="padding: 0 0;" icon="el-icon-link" :disabled="!row.isActive" @click="viewAgent(row)" />
        </template>
      </el-table-column>
      <el-table-column :label="$t('Monitor Port')" align="left">
        <template slot-scope="{row}">
          <span>{{ row.monitorPort }}</span> <el-button type="text" style="padding: 0 0;" icon="el-icon-link" :disabled="!row.isActive" @click="monitorAgent(row)" />
        </template>
      </el-table-column>
      <el-table-column :label="$t('Last Heartbeat')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.lastHeartbeat | simpleFormat }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Is Active')" align="left">
        <template slot-scope="scope">
          <el-tag v-if="scope.row.isActive" type="success" size="small">{{ $t('Active') }}</el-tag>
          <el-tag v-else type="info" size="small">{{ $t('Inactive') }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Creation Time')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.creationTime | simpleFormat }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Delete')" align="center" width="100">
        <template slot-scope="{row}">
          <el-button type="danger" size="mini" @click="removeAgent(row)">
            {{ $t('Delete') }}
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog :title="$t('Agent')" :visible.sync="formVisible">
      <el-form ref="agentForm" :model="form" label-position="left" label-width="120px" style="width: 600px;" :rules="rules">
        <el-form-item :label="$t('Agent')">
          <el-select v-model="form.agentId" style="width:180px;" prop="agentId">
            <el-option
              v-for="agent in agents"
              :key="agent.id"
              :label="agent.name"
              :value="agent.id"
            />
          </el-select>
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="resetAgent">{{ $t('Reset') }}</el-button>
        <el-button type="primary" :disabled="submiting" :loading="submiting" @click.native.prevent="submitAgent">{{ $t('Submit') }}</el-button>
      </div>
    </el-dialog>

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
import { getScene, getTasksOfScene, getAgentsOfScene, addAgent, removeAgent } from '@/api/stress-scene'
import { switchBaseline, addTask } from '@/api/stress-task'
import { getAgentOptions } from '@/api/agent'
import request from '@/utils/request'

export default {
  data() {
    return {
      detail: null,
      tasks: [],
      agentsOfScene: [],
      agents: [],
      formVisible: false,
      submiting: false,
      form: {
        agentId: ''
      },
      rules: {
        agentId: [{ required: true, message: 'Please select an agent', trigger: 'change' }]
      }
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
      getTasksOfScene(id).then(response => {
        this.tasks = response
      })
      getAgentsOfScene(id).then(response => {
        this.agentsOfScene = response
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
      return 'http://' + window.location.host + '/api/callbacks/stress-test?sceneId=' + (this.detail && this.detail.id) + '&caller=test'
    },
    monitor(row) {
      var url = window.location.protocol + '//' + window.location.hostname + ':8080/?id=' + row.id
      window.open(url)

      this.agentsOfScene.forEach((item) => {
        if (item.isActive) {
          var agentUrl = 'http://' + item.agentAddress + ':' + item.monitorPort
          window.open(agentUrl)
        }
      })
    },
    addAgent() {
      this.submiting = false
      this.formVisible = true
      getAgentOptions().then(response => {
        this.agents = response
      })
    },
    submitAgent() {
      const id = this.$route.params.id
      this.$refs.agentForm.validate(valid => {
        if (valid) {
          this.submiting = true
          addAgent(id, this.form).then(() => {
            this.$message({
              type: 'success',
              message: 'Submit success!'
            })
            this.refreshAgents()
            this.formVisible = false
            this.submiting = false
          })
        } else {
          return false
        }
      })
    },
    removeAgent(row) {
      const id = this.$route.params.id
      removeAgent(id, row.id).then(() => {
        this.$message({
          type: 'success',
          message: 'Delete success!'
        })
        this.refreshAgents()
      })
    },
    refreshAgents() {
      const id = this.$route.params.id
      getAgentsOfScene(id).then(response => {
        this.agentsOfScene = response
      })
    },
    viewAgent(row) {
      var url = 'http://' + row.agentAddress + ':' + row.agentPort + '/agent'
      window.open(url)
    },
    monitorAgent(row) {
      var url = 'http://' + row.agentAddress + ':' + row.monitorPort
      window.open(url)
    },
    resetAgent() {
      this.$refs.agentForm.resetFields()
    },
    refresh() {
      const id = this.$route.params.id
      getTasksOfScene(id).then(response => {
        this.tasks = response
      })
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
    "Description": "Description:",
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
    "Schedule": "Schedule",
    "Agent": "Agent",
    "Address": "Address",
    "Port": "Port",
    "Monitor Port": "Monitor Port",
    "Last Heartbeat": "Last Heartbeat",
    "Is Active": "Is Active",
    "Creation Time": "Creation Time",
    "Active": "Active",
    "Inactive": "Inactive",
    "Add": "Add",
    "Submit": "Submit",
    "Delete": "Delete",
    "Reset": "Reset"
  },
  "zh": {
    "Scene Details": "场景详情",
    "Id": "Id:",
    "Name": "名称:",
    "Description": "描述:",
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
    "Schedule": "定时任务",
    "Agent": "代理终端",
    "Address": "访问地址",
    "Port": "访问端口",
    "Monitor Port": "监控端口",
    "Last Heartbeat": "上次心跳",
    "Is Active": "是否活跃",
    "Creation Time": "创建时间",
    "Active": "活跃",
    "Inactive": "不活跃",
    "Add": "添加",
    "Submit": "提交",
    "Delete": "删除",
    "Reset": "重置"
  }
}
</i18n>
