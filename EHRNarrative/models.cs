using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EHRNarrative
{
   
        public class Collection
        {
            public Dialog dialog { get; set; }
            public IEnumerable<Group> groups { get; set; }
            public IEnumerable<Subgroup> subgroups { get; set; }
            public IEnumerable<Element> elements { get; set; }
            public IEnumerable<ComplaintGroup> complaintgroups { get; set; }
            public Complaint complaint { get; set; }
            public IEnumerable<Group_Complaints> group_complaints { get; set; }
            public IEnumerable<Group_Complaint_Groups> group_complaint_groups { get; set; }
            public IEnumerable<Element_Complaints> element_complaints { get; set; }
            public IEnumerable<Element_Complaint_Groups> element_complaint_groups { get; set; }
        }

        public class Dialog
        {
            public int Id { get; set; }
            public string Name { get; set; }

            public IEnumerable<Group> Groups(Collection data)
            {
                return data.groups.Where(x => x.Dialog_id == this.Id);
            }
            public IEnumerable<Group> GroupsForComplaint(Collection data)
            {
                return this.Groups(data).Where(
                           x => x.All_complaints
                        || x.Complaints(data).Contains(data.complaint.Id)
                        || x.ComplaintGroups(data).Contains(data.complaint.Complaintgroup_id));
            }
        }

        public class Group
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Dialog_id { get; set; }

            public List<int> Complaints(Collection data)
            {
                return data.group_complaints.Where(x => x.Group_id == this.Id).Select(x => x.Complaint_id).ToList();
            }
            public List<int> ComplaintGroups(Collection data)
            {
                return data.group_complaint_groups.Where(x => x.Group_id == this.Id).Select(x => x.Complaint_group_id).ToList();
            }

            public IEnumerable<Subgroup> Subgroups(Collection data)
            {
                return data.subgroups.Where(x => x.Group_id == this.Id);
            }
            public IEnumerable<Element> Elements(Collection data)
            {
                return data.elements.Where(x => x.Group_id == this.Id);
            }

            public bool All_complaints { get; set; }

            public IEnumerable<Element> ElementsForComplaint(Collection data)
            {
                return this.Elements(data).Where(
                           x => x.All_complaints
                        || x.Complaints(data).Contains(data.complaint.Id)
                        || x.ComplaintGroups(data).Contains(data.complaint.Complaintgroup_id));
            }
        }
        public class Subgroup
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Group_id { get; set; }

            public IEnumerable<Element> Elements(Collection data)
            {
                return data.elements.Where(x => x.Subgroup_id == this.Id);
            }

        }
        public class Element
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Group_id { get; set; }
            public int Subgroup_id { get; set; }
            public List<int> Complaints(Collection data)
            {
                return data.element_complaints.Where(x => x.Element_id == this.Id).Select(x => x.Complaint_id).ToList();
            }
            public List<int> ComplaintGroups(Collection data)
            {
                return data.element_complaint_groups.Where(x => x.Element_id == this.Id).Select(x => x.Complaint_group_id).ToList();
            }

            public bool All_complaints { get; set; }
            public string EHR_keyword { get; set; }
            public bool Is_present_normal { get; set; }
            public bool Default_present { get; set; }
            public string Present_text { get; set; }
            public string Not_present_text { get; set; }
        }

        public class ComplaintGroup
        {
            public int Complaintgroup_id { get; set; }
            public string Name { get; set; }
            public IList<Complaint> Complaint { get; set; }
        }
        public class Complaint
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Complaintgroup_id { get; set; }
        }

        public class Group_Complaints
        {
            public int Group_id { get; set; }
            public int Complaint_id { get; set; }
        }
        public class Group_Complaint_Groups
        {
            public int Group_id { get; set; }
            public int Complaint_group_id { get; set; }
        }
        public class Element_Complaints
        {
            public int Element_id { get; set; }
            public int Complaint_id { get; set; }
        }
        public class Element_Complaint_Groups
        {
            public int Element_id { get; set; }
            public int Complaint_group_id { get; set; }
        }
    
}
