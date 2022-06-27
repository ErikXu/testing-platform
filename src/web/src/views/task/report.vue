<template>
  <div class="app-container">
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
          <span>{{ scope.row.previous }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Baseline')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.baseline }}</span>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { getReportOfTask } from '@/api/task'

export default {
  data() {
    return {
      report: null
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    fetchData() {
      const id = this.$route.params.id
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
    "Item": "Item",
    "Current": "Current",
    "Previous": "Previous",
    "Baseline": "Baseline"
  },
  "zh": {
    "Item": "项目",
    "Current": "当前结果",
    "Previous": "上次结果",
    "Baseline": "基线结果"
  }
}
</i18n>
