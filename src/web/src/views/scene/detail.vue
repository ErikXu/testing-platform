<template>
  <div class="app-container">
    <h3>Scene Details</h3>
    <el-card class="box-card">
      <el-form label-width="40%" size="mini">
        <el-form-item label="Id:">
          <span>{{ detail && detail.id }}</span>
        </el-form-item>
        <el-form-item label="Name:">
          <span>{{ detail && detail.name }}</span>
        </el-form-item>
        <el-form-item label="Url:">
          <span>{{ detail && detail.url }}</span>
        </el-form-item>
        <el-form-item label="Method:">
          <span>{{ detail && detail.method }}</span>
        </el-form-item>
        <el-form-item label="Thread:">
          <span>{{ detail && detail.thread }}</span>
        </el-form-item>
        <el-form-item label="Connection:">
          <span>{{ detail && detail.connection }}</span>
        </el-form-item>
        <el-form-item label="Duration:">
          <span>{{ detail && (detail.duration + detail.unit) }}</span>
        </el-form-item>
      </el-form>
    </el-card>
    <h3>Task List</h3>
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
      <el-table-column label="ID" align="left" width="220">
        <template slot-scope="scope">
          <span>{{ scope.row.id }}</span>
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
    }
  }
}
</script>
