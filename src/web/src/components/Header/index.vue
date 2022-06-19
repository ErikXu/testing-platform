<template>
  <header>
    <el-dropdown
      trigger="click"
      class="nav-dropdown nav-lang"
      :class="{ 'is-active': langDropdownVisible }"
    >
      <span>
        {{ displayedLang }}
        <i class="el-icon-arrow-down el-icon--right" />
      </span>
      <el-dropdown-menu
        slot="dropdown"
        class="nav-dropdown-list"
        @input="handleLangDropdownToggle"
      >
        <el-dropdown-item
          v-for="(value, key) in langs"
          :key="key"
          @click.native="switchLang(key)"
        >
          {{ value }}
        </el-dropdown-item>
      </el-dropdown-menu>
    </el-dropdown>
  </header>
</template>

<script>
import { getLocale, setLocale } from '@/i18n'
export default {
  name: 'Header',
  data() {
    return {
      lang: getLocale(),
      langDropdownVisible: false,
      langs: {
        'zh': '中文',
        'en': 'English'
      }
    }
  },
  computed: {
    displayedLang() {
      return this.langs[this.lang] || '中文'
    }
  },
  methods: {
    switchLang(targetLang) {
      if (this.lang === targetLang) return
      this.lang = targetLang
      setLocale(targetLang)
      location.reload()
    },
    handleLangDropdownToggle(visible) {
      this.langDropdownVisible = visible
    }
  }
}
</script>

<style lang="scss" scoped>
header{
  width: 100%;
  position: relative;
  height: 60px;
  .nav-dropdown {
    margin-bottom: 6px;
    padding-left: 18px;
    width: 100px;
    position: absolute;
    right: 0;
    top: 50%;
    transform: translateY(-50%);
    span {
      display: block;
      width: 100%;
      font-size: 16px;
      color: #888;
      line-height: 40px;
      transition: .2s;
      padding-bottom: 6px;
      user-select: none;

      &:hover {
         cursor: pointer;
       }
    }

    i {
      transition: .2s;
      font-size: 12px;
      color: #979797;
      transform: translateY(-2px);
    }

    .is-active {
      span, i {
        color: #409EFF;
      }
      i {
        transform: rotateZ(180deg) translateY(3px);
      }
    }

    &:hover {
      span, i {
        color: #409EFF;
      }
    }
  }
}
</style>
