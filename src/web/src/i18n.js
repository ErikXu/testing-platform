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

/**
 * locale key to router path prefix keypair
 */
export const LOCALE_MAP = {
  en: '/en',
  zh: '/zh'
}

const navigatorLangMAP = {
  en: 'en-US',
  zh: 'zh-CN'
}

export const getLocale = () => {
  const path = location.pathname
  let pathLang = ''
  Object.keys(LOCALE_MAP).forEach(targetLang => {
    // get locale from url first
    const langPrefix = LOCALE_MAP[targetLang]
    if (path.startsWith(langPrefix)) {
      pathLang = targetLang
    }
  })
  if (pathLang) {
    return pathLang
  }
  if (localStorage.getItem(langguageKey)) {
    return localStorage.getItem(langguageKey)
  }
  return process.env.VUE_APP_I18N_LOCALE
}

/**
 * 设置语言，目前支持中文和英文
 * @param {String} lang zh | en
 */
export const setLocale = (lang) => {
  localStorage.setItem(langguageKey, lang)
}

export default new VueI18n({
  locale: getLocale() || 'en',
  fallbackLocale: getLocale() || 'en',
  messages: loadLocaleMessages()
})

export const initDefaultLocale = () => {
  const path = location.pathname
  if (path === '/') {
    let pathLang = ''
    Object.keys(LOCALE_MAP).forEach(targetLang => {
      // get locale from url first
      var langPrefix = LOCALE_MAP[targetLang]
      if (path.startsWith(langPrefix)) {
        pathLang = targetLang
      }
    })
    if (pathLang) return
    const storagedLang = localStorage.getItem(langguageKey)
    if (storagedLang) {
      const routerPrefix = LOCALE_MAP[storagedLang]
      if (routerPrefix) {
        window.location.replace(location.protocol + '//' + location.host + routerPrefix)
        return
      }
    }

    if (Object.values(navigatorLangMAP).includes(navigator.language)) {
      window.location.replace(location.protocol + '//' + location.host + '/' + navigator.language)
      return
    }
    window.location.replace(location.protocol + '//' + location.host)
  } else if (!Object.values(LOCALE_MAP).includes(path)) {
    window.location.replace(location.protocol + '//' + location.host + '/' + getLocale())
  }
}
