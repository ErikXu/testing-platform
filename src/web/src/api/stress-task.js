import request from '@/utils/request'

export function getTaskList() {
  return request({
    url: '/api/stress-tasks',
    method: 'get'
  })
}

export function getReportOfTask(id) {
  return request({
    url: `/api/stress-tasks/${id}/report`,
    method: 'get'
  })
}

export function getSceneOfTask(id) {
  return request({
    url: `/api/stress-tasks/${id}/scene`,
    method: 'get'
  })
}

export function addTask(sceneId) {
  return request({
    url: `/api/stress-tasks?sceneId=${sceneId}`,
    method: 'post'
  })
}
