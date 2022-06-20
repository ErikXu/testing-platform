import request from '@/utils/request'

export function getDevice() {
  return request({
    url: '/api/devices',
    method: 'get'
  })
}

export function refreshDevice() {
  return request({
    url: '/api/devices',
    method: 'put'
  })
}
