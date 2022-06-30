<template>
  <div class="app-container">
    <h3>{{ $t('Api Test') }}</h3>
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
      <el-table-column :label="$t('Description')" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.description }}</span>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Collection')" align="center" width="120">
        <template slot-scope="{row}">
          <el-row>
            <el-col :span="12"><el-button type="text" @click.native.prevent="viewCollection(row)">{{ $t('View') }}</el-button></el-col>
            <el-col :span="12">
              <el-upload
                :multiple="false"
                :auto-upload="true"
                :show-file-list="false"
                accept=".json"
                :action="'/api/api-scenes/' + row.id + '/collection'"
                :on-success="uploadSuccess"
              >
                <el-button type="text">{{ $t('Upload') }}</el-button>
              </el-upload>
            </el-col>
          </el-row>
        </template>
      </el-table-column>
      <el-table-column :label="$t('Environment')" align="center" width="120">
        <template slot-scope="{row}">
          <el-row>
            <el-col :span="12"><el-button type="text" @click.native.prevent="viewEnvironment(row)">{{ $t('View') }}</el-button></el-col>
            <el-col :span="12">
              <el-upload
                :multiple="false"
                :auto-upload="true"
                :show-file-list="false"
                accept=".json"
                :action="'/api/api-scenes/' + row.id + '/environment'"
                :on-success="uploadSuccess"
              >
                <el-button type="text">{{ $t('Upload') }}</el-button>
              </el-upload>
            </el-col>
          </el-row>
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
        <el-form-item :label="$t('Description')">
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
import { getSceneList, addScene } from '@/api/api-scene'
import { addTask } from '@/api/api-task'

export default {
  name: 'ApiScene',
  data() {
    return {
      list: [],
      conllectionFileList: [],
      formVisible: false,
      submiting: false,
      form: {
        name: 'My Scene',
        description: 'This is my scene'
      },
      rules: {
        name: [{ required: true, message: 'Please input scene name', trigger: 'change' }]
      }
    }
  },
  created() {
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
        this.$router.push({ name: 'api-scene-detail', params: { id: row.id }})
      })
    },
    detail(row) {
      this.$router.push({ name: 'api-scene-detail', params: { id: row.id }})
    },
    uploadSuccess() {
      this.$message({
        type: 'success',
        message: 'Upload success!'
      })
      this.fetchData()
    },
    viewCollection(row) {
      alert('View ' + row.name + "'s conllection")
    },
    viewEnvironment(row) {
      alert('View ' + row.name + "'s environment")
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
    "Api Test": "Api Test",
    "Create": "Create",
    "Name": "Name",
    "Description": "Description",
    "Collection": "Collection",
    "View": "View",
    "Upload": "Upload",
    "Environment": "Environment",
    "Operation": "Operation",
    "Run": "Run",
    "Detail": "Detail",
    "Scene": "Scene",
    "Submit": "Submit",
    "Reset": "Reset"
  },
  "zh": {
    "Api Test": "接口测试",
    "Create": "创建",
    "Name": "名称",
    "Description": "描述",
    "Collection": "集合",
    "View": "查看",
    "Upload": "上传",
    "Environment": "环境变量",
    "Operation": "操作",
    "Run": "运行",
    "Detail": "详情",
    "Scene": "场景",
    "Submit": "提交",
    "Reset": "重置"
  }
}
</i18n>
