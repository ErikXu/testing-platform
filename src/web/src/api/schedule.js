import request from '@/utils/request'

export function getScheduleList() {
  return request({
    url: '/api/schedules',
    method: 'get'
  })
}

export function switchEnabled(id) {
  return request({
    url: `/api/schedules/${id}/enabled`,
    method: 'patch'
  })
}

export function addSchedule(form) {
  return request({
    url: '/api/schedules',
    method: 'post',
    data: form
  })
}
