<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script>
import Header from '@/components/Header/index.vue'

export default {
  name: 'App',
  components: {
    Header
  }
}
</script>
