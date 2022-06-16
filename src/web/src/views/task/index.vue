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
      <el-table-column label="Scene" align="left" width="140">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Status" align="center" width="80">
        <template slot-scope="scope">
          <el-tag v-if="scope.row.status === 0" type="info" size="small">Waiting</el-tag>
          <el-tag v-else-if="scope.row.status === 1" type="primary" size="small">Runing</el-tag>
          <el-tag v-else-if="scope.row.status === 2" type="success" size="small">Done</el-tag>
          <el-tag v-else-if="scope.row.status === 3" type="danger" size="small">Error</el-tag>
        </template>
      </el-table-column>
      <el-table-column label="IsBaseline" align="left" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.isBaseline }}</span>
        </template>
      </el-table-column>
      <el-table-column label="StartTime" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.startRunningTime | simpleFormat }}</span>
        </template>
      </el-table-column>
      <el-table-column label="EndTime" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.endRunningTime | simpleFormat }}</span>
        </template>
      </el-table-column>
      <el-table-column label="CreationTime" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.creationTime | simpleFormat }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Report" align="center" width="90">
        <template slot-scope="{row}">
          <el-button type="primary" size="mini" @click="report(row)">
            View
          </el-button>
        </template>
      </el-table-column>
    </el-table>
  </div>
</template>

<script>
import { getTaskList } from '@/api/task'

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
