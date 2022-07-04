<template>
  <div class="app-container">
    <h3>{{ $t('Scene Details') }}</h3>
    <el-card class="box-card">
      <el-form label-width="40%" size="mini">
        <el-form-item :label="$t('Id')">
          <span>{{ scene && scene.id }}</span>
        </el-form-item>
        <el-form-item :label="$t('Name')">
          <span>{{ scene && scene.name }}</span>
        </el-form-item>
        <el-form-item :label="$t('Description')">
          <span>{{ scene && scene.description }}</span>
        </el-form-item>
      </el-form>
    </el-card>
  </div>
</template>

<script>
import { getSceneOfTask } from '@/api/api-task'
export default {
  data() {
    return {
      scene: null,
      report: null
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    fetchData() {
      const id = this.$route.params.id
      getSceneOfTask(id).then(response => {
        this.scene = response
      })
    }
  }
}
</script>

<i18n>
{
  "en": {
    "Scene Details": "Scene Details"
  },
  "zh": {
    "Scene Details": "场景信息"
  }
}
</i18n>
