<template>
  <div class="app-container">
    <h3>{{ $t('Scene Details') }}</h3>
    <el-card class="box-card">
      <el-form label-width="40%" size="mini">
        <el-form-item :label="$t('Id')">
          <span>{{ scene && scene.id }}</span>
        </el-form-item>
        <el-form-item :label="$t('Name')">
          <span>{{ scene && scene.name }}</span>
        </el-form-item>
        <el-form-item :label="$t('Url')">
          <span>{{ scene && scene.url }}</span>
        </el-form-item>
        <el-form-item :label="$t('Method')">
          <span>{{ scene && scene.method }}</span>
        </el-form-item>
        <el-form-item :label="$t('Thread')">
          <span>{{ scene && scene.thread }}</span>
        </el-form-item>
        <el-form-item :label="$t('Connection')">
          <span>{{ scene && scene.connection }}</span>
        </el-form-item>
        <el-form-item :label="$t('Duration')">
          <span>{{ scene && (scene.duration + scene.unit) }}</span>
        </el-form-item>
      </el-form>
    </el-card>
    <h3>{{ $t('Report') }}</h3>
    <el-table
      :data="report && report.items"
      border
      fit
      highlight-current-row
    >
      <el-table-column :label="$t('Item')" align="left" width="140">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Current')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.current }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Previous')" align="left">
        <template slot-scope="scope">
          <el-tooltip v-if="scope.row.previousToCurrent < 0" :content="$t('Current is better')" placement="top">
            <span style="color:#67C23A">{{ scope.row.previous }}</span>
          </el-tooltip>
          <el-tooltip v-else-if="scope.row.previousToCurrent > 0" :content="$t('Current is worse')" placement="top">
            <span style="color:#F56C6C">{{ scope.row.previous }}</span>
          </el-tooltip>
          <span v-else>{{ scope.row.previous }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Baseline')" align="left">
        <template slot-scope="scope">
          <el-tooltip v-if="scope.row.baselineToCurrent < 0" :content="$t('Current is better')" placement="top">
            <span style="color:#67C23A">{{ scope.row.baseline }}</span>
          </el-tooltip>
          <el-tooltip v-else-if="scope.row.baselineToCurrent > 0" :content="$t('Current is worse')" placement="top">
            <span style="color:#F56C6C">{{ scope.row.baseline }}</span>
          </el-tooltip>
          <span v-else>{{ scope.row.baseline }}</span>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { getSceneOfTask, getReportOfTask } from '@/api/stress-task'

export default {
  data() {
    return {
      scene: null,
      report: null
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    fetchData() {
      const id = this.$route.params.id
      getSceneOfTask(id).then(response => {
        this.scene = response
      })
      getReportOfTask(id).then(response => {
        this.report = response
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
    "Thread": "Thread:",
    "Connection": "Connection:",
    "Duration": "Duration:",
    "Report": "Report",
    "Item": "Item",
    "Current": "Current",
    "Previous": "Previous",
    "Baseline": "Baseline",
    "Current is better": "Current is better",
    "Current is worse": "Current is worse"
  },
  "zh": {
    "Scene Details": "场景信息",
    "Id": "Id:",
    "Name": "名称:",
    "Url": "Url:",
    "Method": "Method:",
    "Thread": "线程数:",
    "Connection": "连接数:",
    "Duration": "持续时间:",
    "Report": "报告详情",
    "Item": "项目",
    "Current": "当前结果",
    "Previous": "上次结果",
    "Baseline": "基线结果",
    "Current is better": "当前更优",
    "Current is worse": "当前更差"
  }
}
</i18n>
