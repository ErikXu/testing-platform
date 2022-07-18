import request from '@/utils/request'

export function getMethodList() {
  return request({
    url: '/api/stress-scenes/method',
    method: 'get'
  })
}

export function getUnitList() {
  return request({
    url: '/api/stress-scenes/unit',
    method: 'get'
  })
}

export function getContentTypeList() {
  return request({
    url: '/api/stress-scenes/content-type',
    method: 'get'
  })
}

export function getSceneList() {
  return request({
    url: '/api/stress-scenes',
    method: 'get'
  })
}

export function getStressSceneOptions() {
  return request({
    url: '/api/stress-scenes/options',
    method: 'get'
  })
}

export function getScene(id) {
  return request({
    url: `/api/stress-scenes/${id}`,
    method: 'get'
  })
}

export function getTasksOfScene(id) {
  return request({
    url: `/api/stress-scenes/${id}/tasks`,
    method: 'get'
  })
}

export function getAgentsOfScene(id) {
  return request({
    url: `/api/stress-scenes/${id}/agents`,
    method: 'get'
  })
}

export function addScene(form) {
  return request({
    url: '/api/stress-scenes',
    method: 'post',
    data: form
  })
}

export function addAgent(id, form) {
  return request({
    url: `/api/stress-scenes/${id}/agents`,
    method: 'post',
    data: form
  })
}

export function removeAgent(id, agentId) {
  return request({
    url: `/api/stress-scenes/${id}/agents?agentId=${agentId}`,
    method: 'delete'
  })
}
