import request from '@/utils/request'

export function getSceneList() {
  return request({
    url: '/api/api-scenes',
    method: 'get'
  })
}

export function addScene(form) {
  return request({
    url: '/api/api-scenes',
    method: 'post',
    data: form
  })
}
