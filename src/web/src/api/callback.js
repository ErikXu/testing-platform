import request from '@/utils/request'

export function getCallback() {
  return request({
    url: '/api/callbacks',
    method: 'get'
  })
}

export function switchStressTest(id) {
  return request({
    url: `/api/callbacks/${id}/stress-test`,
    method: 'patch'
  })
}

export function switchApiTest(id) {
  return request({
    url: `/api/callbacks/${id}/api-test`,
    method: 'patch'
  })
}
