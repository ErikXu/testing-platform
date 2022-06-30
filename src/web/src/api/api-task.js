import request from '@/utils/request'

export function getTaskList() {
  return request({
    url: '/api/api-tasks',
    method: 'get'
  })
}

export function addTask(sceneId) {
  return request({
    url: `/api/api-tasks?sceneId=${sceneId}`,
    method: 'post'
  })
}
