import request from '@/utils/request'

export function getSceneList() {
  return request({
    url: '/api/api-scenes',
    method: 'get'
  })
}

export function getApiSceneOptions() {
  return request({
    url: '/api/api-scenes/options',
    method: 'get'
  })
}

export function getScene(id) {
  return request({
    url: `/api/api-scenes/${id}`,
    method: 'get'
  })
}

export function getTasksOfScene(id) {
  return request({
    url: `/api/api-scenes/${id}/tasks`,
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
