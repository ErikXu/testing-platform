import request from '@/utils/request'

export function addTask(sceneId) {
  return request({
    url: `/api/api-tasks?sceneId=${sceneId}`,
    method: 'post'
  })
}
