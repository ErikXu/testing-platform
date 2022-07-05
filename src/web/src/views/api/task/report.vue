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
        <el-form-item :label="$t('Description')">
          <span>{{ scene && scene.description }}</span>
        </el-form-item>
      </el-form>
    </el-card>
    <h3>{{ $t('Api List') }}</h3>
    <el-table
      :data="report && report.items"
      border
      fit
      highlight-current-row
    >
      <el-table-column :label="$t('Name')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Method')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.method }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Url')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.url }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Response')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.code }} + {{ scope.row.status }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('ResponseTime')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.responseTime + "ms" }}</span>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { getSceneOfTask, getReportOfTask } from '@/api/api-task'
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
    "Api List": "Api List",
    "Name": "Name",
    "Method": "Method",
    "Url": "Url",
    "Response": "Response",
    "ResponseTime": "ResponseTime"
  },
  "zh": {
    "Scene Details": "场景信息",
    "Api List": "接口列表",
    "Name": "名称",
    "Method": "Method",
    "Url": "Url",
    "Response": "响应",
    "ResponseTime": "响应时间"
  }
}
</i18n>
