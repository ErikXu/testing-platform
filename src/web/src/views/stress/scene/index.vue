<template>
  <div class="app-container">
    <h3>{{ $t('Stress Test') }}</h3>
    <el-row type="flex" style="margin-bottom:10px;" justify="end">
      <el-button size="mini" type="success" @click="create">{{ $t('Create') }}</el-button>
      <el-button size="mini" type="primary" @click="refresh">{{ $t('Refresh') }}</el-button>
    </el-row>
    <el-table
      :data="list"
      border
      fit
      highlight-current-row
      tooltip-effect="light"
    >
      <el-table-column label="#" align="center" width="55">
        <template slot-scope="scope">
          {{ scope.$index + 1 }}
        </template>
      </el-table-column>
      <el-table-column :label="$t('Name')" align="left" width="120">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Description')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.description }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Method')" align="left" width="80">
        <template slot-scope="scope">
          <el-tag v-if="scope.row.method === 'GET'" size="small" effect="dark" color="#61AFFE" style="border-color: #d9ecff;">{{ scope.row.method }}</el-tag>
          <el-tag v-if="scope.row.method === 'POST'" size="small" effect="dark" color="#49C990" style="border-color: #d9ecff;">{{ scope.row.method }}</el-tag>
          <el-tag v-if="scope.row.method === 'PUT'" size="small" effect="dark" color="#FCA130" style="border-color: #d9ecff;">{{ scope.row.method }}</el-tag>
          <el-tag v-if="scope.row.method === 'PATCH'" size="small" effect="dark" color="#50E3C2" style="border-color: #d9ecff;">{{ scope.row.method }}</el-tag>
          <el-tag v-if="scope.row.method === 'DELETE'" size="small" effect="dark" color="#F93E3E" style="border-color: #d9ecff;">{{ scope.row.method }}</el-tag>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Url')" show-overflow-tooltip align="left">
        <template slot-scope="scope">
          <span style="margin-left:5px;">{{ scope.row.url }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Body')" align="center" width="80">
        <template slot-scope="{row}">
          <el-button type="text" @click="view(row)">{{ $t('View') }}</el-button>
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

    <el-dialog :title="$t('Scene')" :visible.sync="formVisible">
      <el-form ref="sceneForm" :model="form" label-position="left" label-width="120px" style="width: 600px;" :rules="rules">
        <el-form-item :label="$t('Name')" prop="name">
          <el-input v-model="form.name" autocomplete="off" />
        </el-form-item>
        <el-form-item :label="$t('Description')">
          <el-input v-model="form.description" type="textarea" />
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
        <el-form-item :label="$t('Content Type')" style="width:180px;">
          <el-select v-model="form.contentType" style="width:180px;">
            <el-option
              v-for="contentType in contentTypeList"
              :key="contentType.id"
              :label="contentType.text"
              :value="contentType.id"
            />
          </el-select>
        </el-form-item>
        <el-form-item :label="$t('Body')">
          <el-input v-model="form.body" type="textarea" />
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

    <el-dialog :title="$t('Body')" :visible.sync="bodyVisible">
      <pre class="language-none">
        <code>{{ body }}</code>
      </pre>
    </el-dialog>
  </div>
</template>

<script>
import { getMethodList, getUnitList, getContentTypeList, getSceneList, addScene } from '@/api/stress-scene'
import { addTask } from '@/api/stress-task'
import 'prismjs/themes/prism-okaidia.css'

export default {
  name: 'StressScene',
  data() {
    return {
      list: [],
      methodList: [],
      unitList: [],
      contentTypeList: [],
      body: '',
      formVisible: false,
      bodyVisible: false,
      submiting: false,
      form: {
        name: 'My Scene',
        description: 'This is my scene',
        url: 'http://localhost/api/tests/get',
        method: 'GET',
        contentType: 'application/json',
        body: '',
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
    })

    getUnitList().then(response => {
      this.unitList = response
    })

    getContentTypeList().then(response => {
      this.contentTypeList = response
    })

    this.fetchData()
  },
  methods: {
    fetchData() {
      getSceneList().then(response => {
        this.list = response
      })
    },
    refresh() {
      this.fetchData()
    },
    create() {
      this.submiting = false
      this.formVisible = true
    },
    view(row) {
      this.body = row.body
      this.bodyVisible = true
    },
    run(row) {
      addTask(row.id).then(() => {
        this.$message({
          type: 'success',
          message: 'Run success!'
        })
        this.$router.push({ name: 'stress-scene-detail', params: { id: row.id }})
      })
    },
    detail(row) {
      this.$router.push({ name: 'stress-scene-detail', params: { id: row.id }})
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
            this.submiting = false
          })
        } else {
          return false
        }
      })
    },
    reset() {
      this.$refs.sceneForm.resetFields()
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Stress Test": "Stress Test - Scene",
    "Create": "Create",
    "Name": "Name",
    "Description": "Description",
    "Url": "Url",
    "Method": "Method",
    "Content Type": "Content Type",
    "Body": "Body",
    "View": "View",
    "Thread": "Thread",
    "Connection": "Connection",
    "Duration": "Duration",
    "Operation": "Operation",
    "Run": "Run",
    "Detail": "Detail",
    "Scene": "Scene",
    "Unit": "Unit",
    "Submit": "Submit",
    "Reset": "Reset",
    "Refresh": "Refresh"
  },
  "zh": {
    "Stress Test": "压力测试 - 场景",
    "Create": "创建",
    "Name": "名称",
    "Description": "描述",
    "Url": "Url",
    "Method": "Method",
    "Content Type": "Content Type",
    "Body": "Body",
    "View": "查看",
    "Thread": "线程数",
    "Connection": "连接数",
    "Duration": "持续时间",
    "Operation": "操作",
    "Run": "运行",
    "Detail": "详情",
    "Scene": "场景",
    "Unit": "单位",
    "Submit": "提交",
    "Reset": "重置",
    "Refresh": "刷新"
  }
}
</i18n>
