<template>
  <div class="app-container">
    <h3>{{ $t('Schedule') }}</h3>
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
      <el-table-column :label="$t('Scene Name')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.sceneName }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Description')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.description }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Type')" align="left">
        <template slot-scope="scope">
          <el-tag v-if="scope.row.testType === 0" type="primary" size="small">{{ $t('Stress Test') }}</el-tag>
          <el-tag v-else-if="scope.row.testType === 1" type="primary" size="small">{{ $t('Api Test') }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Cron Expression')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.cron }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Disabled')" align="left" width="100">
        <template slot-scope="scope">
          <el-switch v-model="scope.row.isDisabled" @change="switchDisabled(scope.row)" />
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
import { getScheduleList, switchDisabled } from '@/api/schedule'

export default {
  name: 'Schedule',
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
      getScheduleList().then(response => {
        this.list = response
      })
    },
    switchDisabled(row) {
      switchDisabled(row.id).then(() => {
        this.fetchData()
      })
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Schedule": "Schedule",
    "Scene Name": "Scene Name",
    "Description": "Description",
    "Type": "Type",
    "Stress Test": "Stress Test",
    "Api Test": "Api Test",
    "Cron Expression": "Cron Expression",
    "Disabled": "Disabled",
    "Creation Time": "Creation Time"
  },
  "zh": {
    "Schedule": "定时任务",
    "Scene Name": "场景",
    "Description": "描述",
    "Type": "类型",
    "Stress Test": "压力测试",
    "Api Test": "接口测试",
    "Cron Expression": "Cron 表达式",
    "Disabled": "禁用",
    "Creation Time": "创建时间"
  }
}
</i18n>
