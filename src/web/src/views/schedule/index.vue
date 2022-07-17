<template>
  <div class="app-container">
    <h3>{{ $t('Schedule') }}</h3>
    <el-row type="flex" style="margin-bottom:10px;" justify="end">
      <el-button size="mini" type="primary" @click="create">{{ $t('Create') }}</el-button>
    </el-row>
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
      <el-table-column :label="$t('Enabled')" align="left" width="100">
        <template slot-scope="scope">
          <el-switch v-model="scope.row.isEnabled" @change="switchEnabled(scope.row)" />
        </template>
      </el-table-column>
      <el-table-column :label="$t('Creation Time')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.creationTime | simpleFormat }}</span>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog :title="$t('Schedule')" :visible.sync="formVisible">
      <el-form ref="scheduleForm" :model="form" label-position="left" label-width="100px" style="width: 600px;" :rules="rules">
        <el-form-item :label="$t('Type')" prop="testType">
          <el-select v-model="form.testType" @change="typeChange">
            <el-option
              v-for="type in types"
              :key="type.value"
              :label="type.label"
              :value="type.value"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="$t('Scene')" prop="sceneId">
          <el-select v-model="form.sceneId">
            <el-option
              v-for="scene in scenes"
              :key="scene.id"
              :label="scene.name"
              :value="scene.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="$t('Cron Expression')" prop="cron">
          <el-input v-model="form.cron" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('Description')" prop="description">
          <el-input v-model="form.description" type="textarea" />
        </el-form-item>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="reset">{{ $t('Reset') }}</el-button>
        <el-button type="primary" :disabled="submiting" :loading="submiting" @click.native.prevent="submit">{{ $t('Submit') }}</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { getScheduleList, switchEnabled, addSchedule } from '@/api/schedule'
import { getStressSceneOptions } from '@/api/stress-scene'
import { getApiSceneOptions } from '@/api/api-scene'

export default {
  name: 'Schedule',
  data() {
    return {
      list: [],
      scenes: [],
      types: [{
        value: 0,
        label: this.$t('Stress Test')
      },
      {
        value: 1,
        label: this.$t('Api Test')
      }
      ],
      formVisible: false,
      submiting: false,
      form: {
        testType: '',
        sceneId: '',
        cron: '* * * * *',
        description: 'This is my schedule'
      }
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
    create() {
      this.submiting = false
      this.formVisible = true
    },
    typeChange(item) {
      if (item === 0) {
        getStressSceneOptions().then(response => {
          this.scenes = response
        })
      } else if (item === 1) {
        getApiSceneOptions().then(response => {
          this.scenes = response
        })
      } else {
        this.scenes = []
      }
    },
    switchEnabled(row) {
      switchEnabled(row.id).then(() => {
        this.fetchData()
      })
    },
    submit() {
      this.$refs.scheduleForm.validate(valid => {
        if (valid) {
          this.submiting = true
          addSchedule(this.form).then(() => {
            this.$message({
              type: 'success',
              message: 'Submit success!'
            })
            this.fetchData()
            this.formVisible = false
            this.submiting = false
          })
        } else {
          return false
        }
      })
    },
    reset() {
      this.$refs.scheduleForm.resetFields()
      this.scenes = []
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Schedule": "Schedule",
    "Create": "Create",
    "Scene Name": "Scene Name",
    "Description": "Description",
    "Type": "Type",
    "Stress Test": "Stress Test",
    "Api Test": "Api Test",
    "Cron Expression": "Cron Expression",
    "Enabled": "Enabled",
    "Creation Time": "Creation Time"
  },
  "zh": {
    "Schedule": "定时任务",
     "Create": "创建",
    "Scene Name": "场景",
    "Description": "描述",
    "Type": "类型",
    "Stress Test": "压力测试",
    "Api Test": "接口测试",
    "Cron Expression": "Cron 表达式",
    "Enabled": "启用",
    "Creation Time": "创建时间"
  }
}
</i18n>
