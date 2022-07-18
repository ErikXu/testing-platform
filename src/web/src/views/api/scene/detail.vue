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
        <el-form-item :label="$t('Callback Address')">
          <span>{{ getCallbackAddress() }}</span> <el-button type="success" size="mini" @click="test">{{ $t('Test') }}</el-button>
        </el-form-item>
      </el-form>
    </el-card>
    <h3>{{ $t('Collection') }}</h3>
    <div v-if="(detail && detail.collectionItems) === null">
      {{ $t('Collection is not uploaded') }}
    </div>
    <div v-else>
      <div v-if="detail.isCollectionInvalid">
        {{ $t('Collection is invalid, please upload again') }}
      </div>
      <div v-else>
        <el-tabs v-model="activeCollection" type="card">
          <el-tab-pane
            v-for="item in detail.collectionItems"
            :key="item.name"
            :label="item.name"
            :name="item.name"
          >
            <el-form label-width="40%" size="mini">
              <el-form-item :label="$t('Url')">
                <span>{{ item.request.url.raw }}</span>
              </el-form-item>
              <el-form-item :label="$t('Method')">
                <span>{{ item.request.method }}</span>
              </el-form-item>
            </el-form>
          </el-tab-pane>
        </el-tabs>
      </div>
    </div>
    <el-upload
      :multiple="false"
      :auto-upload="true"
      :show-file-list="false"
      accept=".json"
      :action="'/api/api-scenes/' + id + '/collection'"
      :on-success="uploadSuccess"
    >
      <el-button type="primary" size="mini" style="margin-top:8px;">{{ $t('Upload') }}</el-button>
    </el-upload>
    <h3>{{ $t('Environment') }}</h3>
    <div v-if="(detail && detail.environmentInfo) === null">
      {{ $t('Environment is not uploaded') }}
    </div>
    <div v-else>
      <div v-if="detail.isEnvironmentInvalid">
        {{ $t('Environment is invalid, please upload again') }}
      </div>
      <div v-else>
        <el-tag>{{ detail && detail.environmentInfo.name }}</el-tag>
        <el-table
          :data="detail.environmentInfo.values"
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
          <el-table-column label="Key" align="left">
            <template slot-scope="scope">
              <span>{{ scope.row.key }}</span>
            </template>
          </el-table-column>
          <el-table-column label="Value" align="left">
            <template slot-scope="scope">
              <span>{{ scope.row.value }}</span>
            </template>
          </el-table-column>
          <el-table-column label="Type" align="left">
            <template slot-scope="scope">
              <span>{{ scope.row.type }}</span>
            </template>
          </el-table-column>
          <el-table-column label="Enabled" align="left">
            <template slot-scope="scope">
              <span>{{ scope.row.enabled }}</span>
            </template>
          </el-table-column>
        </el-table>
      </div>
    </div>
    <el-upload
      :multiple="false"
      :auto-upload="true"
      :show-file-list="false"
      accept=".json"
      :action="'/api/api-scenes/' + id + '/environment'"
      :on-success="uploadSuccess"
    >
      <el-button type="primary" size="mini" style="margin-top:8px;">{{ $t('Upload') }}</el-button>
    </el-upload>
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
import { getScene, getTasksOfScene } from '@/api/api-scene'
import { addTask } from '@/api/api-task'
import request from '@/utils/request'

export default {
  data() {
    return {
      id: null,
      detail: null,
      activeCollection: '',
      tasks: []
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    fetchData() {
      this.id = this.$route.params.id
      getScene(this.id).then(response => {
        this.detail = response
        if (this.detail.collectionItems !== null) {
          this.activeCollection = this.detail.collectionItems[0].name
        }
      })
      getTasksOfScene(this.id).then(response => {
        this.tasks = response
      })
    },
    uploadSuccess() {
      this.$message({
        type: 'success',
        message: 'Upload success!'
      })
      this.fetchData()
    },
    report(row) {
      this.$router.push({ name: 'api-task-report', params: { id: row.id }})
    },
    getCallbackAddress() {
      return 'http://' + window.location.host + '/api/callbacks/api-test?sceneId=' + (this.detail && this.detail.id) + '&caller=test'
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
      this.$router.push({ name: 'api-scene' })
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
    "Callback Address": "Callback Address:",
    "Collection": "Collection",
    "Collection is not uploaded": "Collection is not uploaded!",
    "Collection is invalid, please upload again": "Collection is invalid, please upload again!",
    "Environment": "Environment",
    "Environment is not uploaded": "Environment is not uploaded",
    "Environment is invalid, please upload again": "Environment is invalid, please upload again!",
    "Upload": "Upload",
    "Task List": "Task List",
    "Status": "Status",
    "From": "From",
    "StartTime": "StartTime",
    "EndTime": "EndTime",
    "Creation Time": "Creation Time",
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
    "Description": "描述:",
    "Url": "Url:",
    "Method": "Method:",
    "Callback Address": "回调地址:",
    "Collection": "Collection",
    "Collection is not uploaded": "Collection 未上传！",
    "Collection is invalid, please upload again": "Collection 格式有误，请重新上传！",
    "Environment": "Environment",
    "Environment is not uploaded": "Environment 未上传！",
    "Environment is invalid, please upload again": "Environment 格式有误，请重新上传！",
    "Upload": "上传",
    "Task List": "任务列表",
    "Status": "状态",
    "From": "来源",
    "StartTime": "开始时间",
    "EndTime": "结束时间",
    "Creation Time": "创建时间",
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
