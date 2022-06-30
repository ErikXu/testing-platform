<template>
  <div class="app-container">
    <h3>{{ $t('Stress Test') }}</h3>
    <el-button size="mini" type="primary" @click="create">{{ $t('Create') }}</el-button>
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
      <el-table-column :label="$t('Name')" align="left" width="140">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Url')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.url }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Method')" align="center" width="80">
        <template slot-scope="scope">
          <span>{{ scope.row.method }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Body')" align="center" width="80">
        <template>
          <span>Body</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Thread')" align="center" width="80">
        <template slot-scope="scope">
          <span>{{ scope.row.thread }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Connection')" align="center" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.connection }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Duration')" align="center" width="80">
        <template slot-scope="scope">
          <span>{{ scope.row.duration + scope.row.unit }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Operation')" align="center" width="150">
        <template slot-scope="{row}">
          <el-button type="success" size="mini" @click="run(row)">
            {{ $t('Run') }}
          </el-button>
          <el-button type="primary" size="mini" @click="detail(row)">
            {{ $t('Detail') }}
          </el-button>
        </template>
      </el-table-column>
    </el-table>

    <el-dialog :title="$t('Scene')" :visible.sync="formVisible" @close="reset">
      <el-form ref="sceneForm" :model="form" label-position="left" label-width="100px" style="width: 600px;" :rules="rules">
        <el-form-item :label="$t('Name')" prop="name">
          <el-input v-model="form.name" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('Url')" prop="url">
          <el-col :span="5">
            <el-select v-model="form.method">
              <el-option
                v-for="method in methodList"
                :key="method.id"
                :label="method.text"
                :value="method.id"
              />
            </el-select>
          </el-col>
          <el-col :span="19">
            <el-input v-model="form.url" autocomplete="off" />
          </el-col>
        </el-form-item>
        <el-form-item :label="$t('Thread')" style="width:180px;">
          <el-input-number v-model="form.thread" :min="1" />
        </el-form-item>
        <el-form-item :label="$t('Connection')" style="width:180px;">
          <el-input-number v-model="form.connection" :min="1" />
        </el-form-item>
        <el-form-item :label="$t('Duration')" style="width:180px;">
          <el-input-number v-model="form.duration" :min="1" />
        </el-form-item>
        <el-form-item :label="$t('Unit')">
          <el-select v-model="form.unit" style="width:180px;">
            <el-option
              v-for="unit in unitList"
              :key="unit.id"
              :label="unit.text"
              :value="unit.id"
            />
          </el-select>
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
import { getMethodList, getUnitList, getSceneList, addScene } from '@/api/stress-scene'
import { addTask } from '@/api/stress-task'

export default {
  name: 'StressScene',
  data() {
    return {
      list: [],
      methodList: [],
      unitList: [],
      formVisible: false,
      submiting: false,
      form: {
        name: 'My Scene',
        url: 'http://localhost/api/tests/get',
        method: 'GET',
        thread: 1,
        connection: 1,
        duration: 5,
        unit: 's'
      },
      rules: {
        name: [{ required: true, message: 'Please input scene name', trigger: 'change' }],
        url: [{ required: true, message: 'Please input url', trigger: 'change' }]
      }
    }
  },
  created() {
    getMethodList().then(response => {
      this.methodList = response
      getUnitList().then(response => {
        this.unitList = response
      })
    })
    this.fetchData()
  },
  methods: {
    fetchData() {
      getSceneList().then(response => {
        this.list = response
      })
    },
    create() {
      this.submiting = false
      this.formVisible = true
    },
    run(row) {
      addTask(row.id).then(response => {
        this.list = response
        this.$message({
          type: 'success',
          message: 'Run success!'
        })
        this.$router.push({ name: 'stress-scene-detail', params: { id: row.id }})
      })
    },
    detail(row) {
      this.$router.push({ name: 'scene-detail', params: { id: row.id }})
    },
    submit() {
      this.$refs.sceneForm.validate(valid => {
        if (valid) {
          this.submiting = true
          addScene(this.form).then(() => {
            this.$message({
              type: 'success',
              message: 'Submit success!'
            })
            this.fetchData()
            this.formVisible = false
          })
        } else {
          return false
        }
      })
    },
    reset() {
      this.formVisible = false
      this.$refs.sceneForm.resetFields()
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Stress Test": "Stress Test",
    "Create": "Create",
    "Name": "Name",
    "Url": "Url",
    "Method": "Method",
    "Body": "Body",
    "Thread": "Thread",
    "Connection": "Connection",
    "Duration": "Duration",
    "Operation": "Operation",
    "Run": "Run",
    "Detail": "Detail",
    "Scene": "Scene",
    "Unit": "Unit",
    "Submit": "Submit",
    "Reset": "Reset"
  },
  "zh": {
    "Stress Test": "压力测试",
    "Create": "创建",
    "Name": "名称",
    "Url": "Url",
    "Method": "Method",
    "Body": "Body",
    "Thread": "线程数",
    "Connection": "连接数",
    "Duration": "持续时间",
    "Operation": "操作",
    "Run": "运行",
    "Detail": "详情",
    "Scene": "场景",
    "Unit": "单位",
    "Submit": "提交",
    "Reset": "重置"
  }
}
</i18n>
