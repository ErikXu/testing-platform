import request from '@/utils/request'

export function getAgentList() {
  return request({
    url: '/api/agents',
    method: 'get'
  })
}

export function getAgentOptions() {
  return request({
    url: '/api/agents/options',
    method: 'get'
  })
}
