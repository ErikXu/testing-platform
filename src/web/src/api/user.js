import request from '@/utils/request'

export function login(data) {
  return request({
    url: '/api/users/login',
    method: 'post',
    data
  })
}

export function getInfo(token) {
  return request({
    url: '/api/users/info',
    method: 'get',
    params: { token }
  })
}

export function logout() {
  return request({
    url: '/api/users/logout',
    method: 'post'
  })
}
