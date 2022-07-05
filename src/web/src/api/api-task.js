import request from '@/utils/request'

export function getTaskList() {
  return request({
    url: '/api/api-tasks',
    method: 'get'
  })
}

export function getTask(id) {
  return request({
    url: `/api/api-tasks/${id}`,
    method: 'get'
  })
}

export function getReportOfTask(id) {
  return request({
    url: `/api/api-tasks/${id}/report`,
    method: 'get'
  })
}

export function getSceneOfTask(id) {
  return request({
    url: `/api/api-tasks/${id}/scene`,
    method: 'get'
  })
}

export function addTask(sceneId) {
  return request({
    url: `/api/api-tasks?sceneId=${sceneId}`,
    method: 'post'
  })
}
