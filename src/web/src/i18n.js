import Vue from 'vue'
import VueI18n from 'vue-i18n'

Vue.use(VueI18n)

const langguageKey = 'test_platform_lang'

function loadLocaleMessages() {
  const locales = require.context('./locales', true, /[A-Za-z0-9-_,\s]+\.json$/i)
  const messages = {}
  locales.keys().forEach(key => {
    const matched = key.match(/([A-Za-z0-9-_]+)\./i)
    if (matched && matched.length > 1) {
      const locale = matched[1]
      messages[locale] = locales(key)
    }
  })
  return messages
}

export const getLocale = () => {
  if (localStorage.getItem(langguageKey)) {
    return localStorage.getItem(langguageKey)
  }
  return process.env.VUE_APP_I18N_LOCALE
}

/**
 * 设置语言，目前支持中文和英文
 * @param {String} lang zh-CN | en
 */
export const setLocale = (lang) => {
  localStorage.setItem(langguageKey, lang)
}

export default new VueI18n({
  locale: getLocale() || 'en',
  fallbackLocale: getLocale() || 'en',
  messages: loadLocaleMessages()
})
