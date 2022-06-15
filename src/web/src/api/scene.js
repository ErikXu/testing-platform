import request from '@/utils/request'

export function getMethodList() {
  return request({
    url: '/api/scenes/method',
    method: 'get'
  })
}

export function getUnitList() {
  return request({
    url: '/api/scenes/unit',
    method: 'get'
  })
}

export function getSceneList() {
  return request({
    url: '/api/scenes',
    method: 'get'
  })
}

export function getScene(id) {
  return request({
    url: `/api/scenes/${id}`,
    method: 'get'
  })
}

export function getTaskOfScene(id) {
  return request({
    url: `/api/scenes/${id}/tasks`,
    method: 'get'
  })
}

export function addScene(form) {
  return request({
    url: '/api/scenes',
    method: 'post',
    data: form
  })
}
