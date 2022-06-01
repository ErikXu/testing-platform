<template>
  <div class="app-container">
    <el-button size="mini" type="primary" @click="create">Create</el-button>
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
      <el-table-column label="Name" align="left" width="140">
        <template slot-scope="scope">
          <span>{{ scope.row.name }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Url" align="left">
        <template slot-scope="scope">
          <span>{{ scope.row.url }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Method" align="center" width="80">
        <template slot-scope="scope">
          <span>{{ scope.row.method }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Body" align="center" width="80">
        <template>
          <span>Body</span>
        </template>
      </el-table-column>
      <el-table-column label="Thread" align="center" width="80">
        <template slot-scope="scope">
          <span>{{ scope.row.thread }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Connection" align="center" width="100">
        <template slot-scope="scope">
          <span>{{ scope.row.connection }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Duration" align="center" width="80">
        <template slot-scope="scope">
          <span>{{ scope.row.duration + scope.row.unit }}</span>
        </template>
      </el-table-column>
      <el-table-column label="Run" align="center" width="80">
        <el-button type="success" size="mini">
          Run
        </el-button>
      </el-table-column>
    </el-table>

    <el-dialog title="Scene" :visible.sync="formVisible" @close="reset">
      <el-form ref="sceneForm" :model="form" label-position="left" label-width="100px" style="width: 600px;" :rules="rules">
        <el-form-item label="Name" prop="name">
          <el-input v-model="form.name" autocomplete="off" />
        </el-form-item>
        <el-form-item label="Url" prop="url">
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
        <el-form-item label="Thread" style="width:180px;">
          <el-input-number v-model="form.thread" :min="1" />
        </el-form-item>
        <el-form-item label="Connection" style="width:180px;">
          <el-input-number v-model="form.connection" :min="1" />
        </el-form-item>
        <el-form-item label="Duration" style="width:180px;">
          <el-input-number v-model="form.duration" :min="1" />
        </el-form-item>
        <el-form-item label="Unit">
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
        <el-button @click="reset">Reset</el-button>
        <el-button type="primary" :disabled="submiting" :loading="submiting" @click.native.prevent="submit">Submit</el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import { getMethodList, getUnitList, getSceneList, addScene } from '@/api/scene'
export default {
  name: 'Scene',
  data() {
    return {
      list: [],
      methodList: [],
      unitList: [],
      formVisible: false,
      submiting: false,
      form: {
        name: null,
        url: null,
        method: 'GET',
        thread: 1,
        connection: 1,
        duration: 1,
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
    submit() {
      this.$refs.sceneForm.validate(valid => {
        if (valid) {
          this.submiting = true
          addScene(this.form).then(() => {
            this.$message({
              type: 'success',
              message: '创建成功!'
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
      this.$refs.workspaceForm.resetFields()
    }
  }
}
</script>
