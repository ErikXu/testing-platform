<template>
  <div class="app-container">
    <h3>{{ $t('Agent') }}</h3>
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
      <el-table-column :label="$t('Address')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.agentAddress }}</span>
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
    </el-table>
  </div>
</template>

<script>
import { getAgentList } from '@/api/agent'

export default {
  name: 'Agent',
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
      getAgentList().then(response => {
        this.list = response
      })
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Agent": "Agent",
    "Address": "Address",
    "Last Heartbeat": "Last Heartbeat",
    "Is Active": "Is Active",
    "Creation Time": "Creation Time",
    "Active": "Active",
    "Inactive": "Inactive"
  },
  "zh": {
    "Agent": "代理",
    "Address": "地址",
    "Last Heartbeat": "上次心跳",
    "Is Active": "是否活跃",
    "Creation Time": "创建时间",
    "Active": "活跃",
    "Inactive": "不活跃"
  }
}
</i18n>
